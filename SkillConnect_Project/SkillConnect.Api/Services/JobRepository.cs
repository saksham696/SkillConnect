using Microsoft.EntityFrameworkCore;

/// <summary>
/// Data-access layer for Job entities. Sits behind IJobRepository so the
/// controller (and any future caller) depends only on the abstraction -
/// classic Repository pattern / dependency-inversion.
/// </summary>
public class JobRepository : IJobRepository
{
    private readonly SkillConnectDbContext _dbContext;

    public JobRepository(SkillConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private static JobDto ToDto(Job job) => new JobDto
    {
        Id = job.Id,
        Title = job.Title,
        Description = job.Description,
        Company = job.Company,
        Location = job.Location,
        JobType = job.JobType,
        MaximumSalary = job.MaximumSalary,
        MinimumSalary = job.MinimumSalary,
        PostedDate = job.PostedAt,
        DeadLineDate = job.DeadLineDate,
        isActive = job.IsActive,
        PostedById = job.PostedById
    };

    public async Task<PagedResult<JobDto>> GetJobListAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Jobs.Where(j => j.IsActive).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(j =>
                j.Title.ToLower().Contains(term) ||
                j.Company.ToLower().Contains(term) ||
                j.Location.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.PostedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobDto>
        {
            Items = jobs.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<JobDto>> GetJobsByCompanyAsync(int companyUserId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Jobs.Where(j => j.PostedById == companyUserId);

        var totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.PostedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobDto>
        {
            Items = jobs.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<JobDto> GetJobByIdAsync(int id)
    {
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        return job != null ? ToDto(job) : new JobDto();
    }

    public async Task<int> CreateJobAsync(CreateJobRequestDto request, int companyUserId)
    {
        var companyUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == companyUserId);

        var job = new Job
        {
            Title = request.Title,
            Description = request.Description,
            Company = companyUser?.Name ?? "Unknown Company",
            Location = request.Location,
            MinimumSalary = request.MinimumSalary,
            MaximumSalary = request.MaximumSalary,
            JobType = request.JobType,
            PostedAt = DateTime.UtcNow,
            DeadLineDate = request.DeadLineDate,
            IsActive = true,
            PostedById = companyUserId
        };
        _dbContext.Jobs.Add(job);
        await _dbContext.SaveChangesAsync();
        return job.Id;
    }

    public async Task<JobOperationResult> UpdateJobAsync(int id, UpdateJobRequestDto request, int companyUserId)
    {
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
        {
            return new JobOperationResult { Status = JobOperationStatus.NotFound };
        }
        if (job.PostedById != companyUserId)
        {
            return new JobOperationResult { Status = JobOperationStatus.Forbidden };
        }

        job.Title = request.Title;
        job.Description = request.Description;
        job.Location = request.Location;
        job.MinimumSalary = request.MinimumSalary;
        job.MaximumSalary = request.MaximumSalary;
        job.JobType = request.JobType;
        job.DeadLineDate = request.DeadLineDate;
        job.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        return new JobOperationResult { Status = JobOperationStatus.Success, Job = ToDto(job) };
    }

    public async Task<JobOperationResult> DeleteJobAsync(int id, int companyUserId)
    {
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
        {
            return new JobOperationResult { Status = JobOperationStatus.NotFound };
        }
        if (job.PostedById != companyUserId)
        {
            return new JobOperationResult { Status = JobOperationStatus.Forbidden };
        }

        _dbContext.Jobs.Remove(job);
        await _dbContext.SaveChangesAsync();

        return new JobOperationResult { Status = JobOperationStatus.Success };
    }
}
