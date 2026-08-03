public class JobDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal MinimumSalary { get; set; }
    public decimal MaximumSalary { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? JobType { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime DeadLineDate { get; set; }
    public bool isActive { get; set; }
    public int PostedById { get; set; }
}
public class CreateJobRequestDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public decimal MinimumSalary { get; set; }
    public decimal MaximumSalary { get; set; }
    public string JobType { get; set; }
    public DateTime DeadLineDate { get; set; }
}

public class UpdateJobRequestDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public decimal MinimumSalary { get; set; }
    public decimal MaximumSalary { get; set; }
    public string JobType { get; set; }
    public DateTime DeadLineDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Outcome of a job mutation so the controller can map it to the correct
/// HTTP status (404 vs 403) without repositories throwing exceptions for
/// ordinary control flow.
/// </summary>
public enum JobOperationStatus
{
    Success,
    NotFound,
    Forbidden
}

public class JobOperationResult
{
    public JobOperationStatus Status { get; set; }
    public JobDto? Job { get; set; }
}