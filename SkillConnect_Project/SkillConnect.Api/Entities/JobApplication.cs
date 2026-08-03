using System.ComponentModel.DataAnnotations;

public class JobApplication
{
    public int Id { get; set; }

    public Job ApplicationJob { get; set; }

    public int AppliedJobId { get; set; }
    public User AppliedBy { get; set; }
    public int AppliedById { get; set; }
    [Required]

    public DateTime ApplicationDate { get; set; }
    [Required]
    public string Status { get; set; } // e.g., "Pending", "Accepted", "Rejected"
    [Required]
    public bool IsActive { get; set; } // To indicate if the application is still active
    [Required]

    public string CoverLetter { get; set; } // Optional cover letter for the application
    [Required]

    public string ResumePath { get; set; } // Path to the uploaded resume file

    
}