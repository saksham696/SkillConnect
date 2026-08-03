public interface IJobApplicationRepository
{
    /// <summary>Job seeker applies to a job.</summary>
    Task<ApplicationOperationResult> ApplyToJobAsync(int applicantUserId, ApplyJobRequestDto request);

    /// <summary>Job seeker's own application history.</summary>
    Task<PagedResult<JobApplicationDto>> GetApplicationsForApplicantAsync(int applicantUserId, int page, int pageSize);

    /// <summary>Applications for one specific job - only visible to the company that posted it.</summary>
    Task<ApplicationListResult> GetApplicationsForJobAsync(int jobId, int companyUserId);

    /// <summary>All applications across every job posted by the logged-in company (dashboard view).</summary>
    Task<PagedResult<JobApplicationDto>> GetApplicationsForCompanyAsync(int companyUserId, int page, int pageSize);

    Task<ApplicationOperationResult> UpdateApplicationStatusAsync(int applicationId, int companyUserId, string status);
}
