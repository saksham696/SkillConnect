using System.ComponentModel.DataAnnotations;

public class Job
{
    [Required, Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Company { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime PostedAt { get; set; }

    [Required]
    public DateTime DeadLineDate { get; set; }

    [Required]
    public string JobType { get; set; }

    [Required]
    public decimal MinimumSalary { get; set; }

    [Required]
    public decimal MaximumSalary { get; set; }

    [Required]
    public bool IsActive { get; set; }

    public User PostedBy { get; set; }
    public int PostedById { get; set; }
}
