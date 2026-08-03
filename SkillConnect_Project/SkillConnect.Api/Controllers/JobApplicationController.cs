using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for job applications: a JobSeeker applies to a Job, and a
/// Company reviews/manages applicants for the jobs it has posted.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobApplicationController : ControllerBase
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserHelper _currentUserHelper;

    public JobApplicationController(IJobApplicationRepository applicationRepository, ICurrentUserHelper currentUserHelper)
    {
        _applicationRepository = applicationRepository;
        _currentUserHelper = currentUserHelper;
    }

    /// <summary>Job seeker applies to a job (optionally attaching a fresh resume).</summary>
    [HttpPost, Route("apply")]
    [Authorize(Roles = "JobSeeker")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Apply([FromForm] ApplyJobRequestDto request)
    {
        var result = await _applicationRepository.ApplyToJobAsync(_currentUserHelper.userId, request);
        return result.Status switch
        {
            ApplicationOperationStatus.NotFound => NotFound(new { message = result.Message }),
            ApplicationOperationStatus.AlreadyApplied => Conflict(new { message = result.Message }),
            _ => Ok(result.Application)
        };
    }

    /// <summary>Job seeker's own application history.</summary>
    [HttpGet, Route("my-applications")]
    [Authorize(Roles = "JobSeeker")]
    public async Task<PagedResult<JobApplicationDto>> MyApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return await _applicationRepository.GetApplicationsForApplicantAsync(_currentUserHelper.userId, page, pageSize);
    }

    /// <summary>Company view: every applicant for one specific job posting.</summary>
    [HttpGet, Route("job/{jobId}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> ApplicationsForJob(int jobId)
    {
        var result = await _applicationRepository.GetApplicationsForJobAsync(jobId, _currentUserHelper.userId);
        return result.Status switch
        {
            ApplicationOperationStatus.NotFound => NotFound(new { message = "Job not found." }),
            ApplicationOperationStatus.Forbidden => Forbid(),
            _ => Ok(result.Applications)
        };
    }

    /// <summary>Company dashboard: paginated list of every applicant across all of the company's job postings.</summary>
    [HttpGet, Route("company")]
    [Authorize(Roles = "Company")]
    public async Task<PagedResult<JobApplicationDto>> ApplicationsForCompany([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return await _applicationRepository.GetApplicationsForCompanyAsync(_currentUserHelper.userId, page, pageSize);
    }

    /// <summary>Company accepts/rejects an applicant.</summary>
    [HttpPut, Route("{id}/status")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateApplicationStatusRequestDto request)
    {
        var result = await _applicationRepository.UpdateApplicationStatusAsync(id, _currentUserHelper.userId, request.Status);
        return result.Status switch
        {
            ApplicationOperationStatus.NotFound => NotFound(new { message = "Application not found." }),
            ApplicationOperationStatus.Forbidden => Forbid(),
            _ => Ok(result.Application)
        };
    }
}
