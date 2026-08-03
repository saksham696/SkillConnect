public class ApplyJobRequestDto
{
    public int JobId { get; set; }
    public string? CoverLetter { get; set; }

    /// <summary>
    /// Optional - if the applicant does not attach a fresh resume, the
    /// resume already stored on their UserProfile is used instead.
    /// </summary>
    public IFormFile? ResumeFile { get; set; }
}

public class JobApplicationDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public int ApplicantId { get; set; }
    public string ApplicantName { get; set; }
    public string ApplicantEmail { get; set; }
    public DateTime ApplicationDate { get; set; }
    public string Status { get; set; }
    public string CoverLetter { get; set; }
    public string ResumePath { get; set; }
}

public class UpdateApplicationStatusRequestDto
{
    public string Status { get; set; } // "Pending" | "Accepted" | "Rejected"
}

public enum ApplicationOperationStatus
{
    Success,
    NotFound,
    Forbidden,
    AlreadyApplied
}

public class ApplicationOperationResult
{
    public ApplicationOperationStatus Status { get; set; }
    public JobApplicationDto? Application { get; set; }
    public string? Message { get; set; }
}

public class ApplicationListResult
{
    public ApplicationOperationStatus Status { get; set; }
    public List<JobApplicationDto> Applications { get; set; } = new();
}
