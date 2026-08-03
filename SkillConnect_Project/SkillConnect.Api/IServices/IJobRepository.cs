public interface IJobRepository
{
    Task<JobDto> GetJobByIdAsync(int id);

    /// <summary>Public, paginated job listing with optional keyword search.</summary>
    Task<PagedResult<JobDto>> GetJobListAsync(int page, int pageSize, string? search);

    /// <summary>Jobs posted by the currently logged-in company (dashboard "My Jobs").</summary>
    Task<PagedResult<JobDto>> GetJobsByCompanyAsync(int companyUserId, int page, int pageSize);

    Task<int> CreateJobAsync(CreateJobRequestDto request, int companyUserId);

    Task<JobOperationResult> UpdateJobAsync(int id, UpdateJobRequestDto request, int companyUserId);

    Task<JobOperationResult> DeleteJobAsync(int id, int companyUserId);
}
