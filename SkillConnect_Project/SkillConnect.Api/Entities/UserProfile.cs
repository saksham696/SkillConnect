using System.ComponentModel.DataAnnotations;

public class UserProfile
{
  [Required, Key]
  public int Id { get; set; }
  public User User { get; set; }
    public int UserId { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string SkillSet { get; set; } // Comma-separated list of skills
    [Required]
    public string Experience { get; set; } // Comma-separated list of experiences
    [Required]
    public string Education { get; set; } // Comma-separated list of education details
    [Required]
    public string ResumePath { get; set; } // Path to the uploaded resume file
    public string LinkedInProfile { get; set; } // Optional LinkedIn profile URL
    public string GitHubProfile { get; set; } // Optional GitHub profile URL
    [Required]
    public string Bio { get; set; } // Short bio or summary about the user
    [Required]
    public string Location { get; set; } // User's location
    [Required]
    public string ContactNumber { get; set; } // User's contact number
}