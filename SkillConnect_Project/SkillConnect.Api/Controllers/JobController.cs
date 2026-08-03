using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller ("C" in MVC) for everything related to job postings:
/// public listing/search with pagination, and create/update/delete which
/// are restricted to the logged-in Company that owns the posting.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserHelper _currentUserHelper;

    public JobController(IJobRepository jobRepository, ICurrentUserHelper currentUserHelper)
    {
        _jobRepository = jobRepository;
        _currentUserHelper = currentUserHelper;
    }

    [HttpGet("{id}", Name = "GetJobById")]
    public async Task<JobDto> GetJobById(int id)
    {
        return await _jobRepository.GetJobByIdAsync(id);
    }

    /// <summary>Public, paginated job list. Anyone can browse; ?search= filters by title/company/location.</summary>
    [HttpGet("list", Name = "GetJobList")]
    public async Task<PagedResult<JobDto>> GetJobList([FromQuery] int page = 1, [FromQuery] int pageSize = 9, [FromQuery] string? search = null)
    {
        return await _jobRepository.GetJobListAsync(page, pageSize, search);
    }

    /// <summary>Jobs posted by the logged-in company - powers the company dashboard.</summary>
    [HttpGet("my-jobs", Name = "GetMyJobs")]
    [Authorize(Roles = "Company")]
    public async Task<PagedResult<JobDto>> GetMyJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return await _jobRepository.GetJobsByCompanyAsync(_currentUserHelper.userId, page, pageSize);
    }

    [HttpPost, Route("create")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> CreateJob(CreateJobRequestDto request)
    {
        var jobId = await _jobRepository.CreateJobAsync(request, _currentUserHelper.userId);
        var createdJob = await _jobRepository.GetJobByIdAsync(jobId);
        return CreatedAtAction(nameof(GetJobById), new { id = jobId }, createdJob);
    }

    [HttpPut, Route("update/{id}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> UpdateJob(int id, UpdateJobRequestDto request)
    {
        var result = await _jobRepository.UpdateJobAsync(id, request, _currentUserHelper.userId);
        return result.Status switch
        {
            JobOperationStatus.NotFound => NotFound(new { message = "Job not found." }),
            JobOperationStatus.Forbidden => Forbid(),
            _ => Ok(result.Job)
        };
    }

    [HttpDelete, Route("delete/{id}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var result = await _jobRepository.DeleteJobAsync(id, _currentUserHelper.userId);
        return result.Status switch
        {
            JobOperationStatus.NotFound => NotFound(new { message = "Job not found." }),
            JobOperationStatus.Forbidden => Forbid(),
            _ => Ok(new { message = "Job deleted successfully." })
        };
    }
}
