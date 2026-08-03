using Microsoft.EntityFrameworkCore;

/// <summary>
/// Data-access layer for JobApplication entities - handles a job seeker
/// applying to a job, and a company reviewing applicants for their own
/// job postings.
/// </summary>
public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly SkillConnectDbContext _dbContext;
    private readonly IFileUploadHelper _fileUploadHelper;

    public JobApplicationRepository(SkillConnectDbContext dbContext, IFileUploadHelper fileUploadHelper)
    {
        _dbContext = dbContext;
        _fileUploadHelper = fileUploadHelper;
    }

    private static JobApplicationDto ToDto(JobApplication app) => new JobApplicationDto
    {
        Id = app.Id,
        JobId = app.AppliedJobId,
        JobTitle = app.ApplicationJob?.Title ?? string.Empty,
        CompanyName = app.ApplicationJob?.Company ?? string.Empty,
        ApplicantId = app.AppliedById,
        ApplicantName = app.AppliedBy?.Name ?? string.Empty,
        ApplicantEmail = app.AppliedBy?.Email ?? string.Empty,
        ApplicationDate = app.ApplicationDate,
        Status = app.Status,
        CoverLetter = app.CoverLetter,
        ResumePath = app.ResumePath
    };

    public async Task<ApplicationOperationResult> ApplyToJobAsync(int applicantUserId, ApplyJobRequestDto request)
    {
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == request.JobId);
        if (job == null || !job.IsActive)
        {
            return new ApplicationOperationResult { Status = ApplicationOperationStatus.NotFound, Message = "Job not found or no longer accepting applications." };
        }

        var alreadyApplied = await _dbContext.JobApplications
            .AnyAsync(a => a.AppliedJobId == request.JobId && a.AppliedById == applicantUserId);
        if (alreadyApplied)
        {
            return new ApplicationOperationResult { Status = ApplicationOperationStatus.AlreadyApplied, Message = "You have already applied to this job." };
        }

        string resumePath;
        if (request.ResumeFile != null)
        {
            resumePath = await _fileUploadHelper.UploadResumeAsync(request.ResumeFile);
        }
        else
        {
            var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == applicantUserId);
            resumePath = profile?.ResumePath ?? string.Empty;
        }

        var application = new JobApplication
        {
            AppliedJobId = request.JobId,
            AppliedById = applicantUserId,
            ApplicationDate = DateTime.UtcNow,
            Status = "Pending",
            IsActive = true,
            CoverLetter = request.CoverLetter ?? string.Empty,
            ResumePath = resumePath
        };

        _dbContext.JobApplications.Add(application);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(application).Reference(a => a.ApplicationJob).LoadAsync();
        await _dbContext.Entry(application).Reference(a => a.AppliedBy).LoadAsync();

        return new ApplicationOperationResult { Status = ApplicationOperationStatus.Success, Application = ToDto(application) };
    }

    public async Task<PagedResult<JobApplicationDto>> GetApplicationsForApplicantAsync(int applicantUserId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.JobApplications
            .Include(a => a.ApplicationJob)
            .Include(a => a.AppliedBy)
            .Where(a => a.AppliedById == applicantUserId);

        var totalCount = await query.CountAsync();
        var apps = await query
            .OrderByDescending(a => a.ApplicationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobApplicationDto>
        {
            Items = apps.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ApplicationListResult> GetApplicationsForJobAsync(int jobId, int companyUserId)
    {
        var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        if (job == null)
        {
            return new ApplicationListResult { Status = ApplicationOperationStatus.NotFound };
        }
        if (job.PostedById != companyUserId)
        {
            return new ApplicationListResult { Status = ApplicationOperationStatus.Forbidden };
        }

        var apps = await _dbContext.JobApplications
            .Include(a => a.ApplicationJob)
            .Include(a => a.AppliedBy)
            .Where(a => a.AppliedJobId == jobId)
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();

        return new ApplicationListResult
        {
            Status = ApplicationOperationStatus.Success,
            Applications = apps.Select(ToDto).ToList()
        };
    }

    public async Task<PagedResult<JobApplicationDto>> GetApplicationsForCompanyAsync(int companyUserId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.JobApplications
            .Include(a => a.ApplicationJob)
            .Include(a => a.AppliedBy)
            .Where(a => a.ApplicationJob.PostedById == companyUserId);

        var totalCount = await query.CountAsync();
        var apps = await query
            .OrderByDescending(a => a.ApplicationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<JobApplicationDto>
        {
            Items = apps.Select(ToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ApplicationOperationResult> UpdateApplicationStatusAsync(int applicationId, int companyUserId, string status)
    {
        var app = await _dbContext.JobApplications
            .Include(a => a.ApplicationJob)
            .Include(a => a.AppliedBy)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (app == null)
        {
            return new ApplicationOperationResult { Status = ApplicationOperationStatus.NotFound };
        }
        if (app.ApplicationJob.PostedById != companyUserId)
        {
            return new ApplicationOperationResult { Status = ApplicationOperationStatus.Forbidden };
        }

        app.Status = status;
        await _dbContext.SaveChangesAsync();

        return new ApplicationOperationResult { Status = ApplicationOperationStatus.Success, Application = ToDto(app) };
    }
}
