using System.ComponentModel.DataAnnotations;

public class CreateUserProfileRequestDto
{
    [Required(ErrorMessage = "FullName is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "SkillSet is required")]
    public string? SkillSet { get; set; }

    [Required(ErrorMessage = "Experience is required")]
    public string? Experience { get; set; }

    [Required(ErrorMessage = "Education is required")]
    public string? Education { get; set; }

    [Required(ErrorMessage = "Resume is required")]
    public IFormFile? ResumeFile { get; set; }

    public string? LinkedInProfile { get; set; }

    public string? GitHubProfile { get; set; }

    [Required(ErrorMessage = "Bio is required")]
    [StringLength(500, ErrorMessage = "Bio must not exceed 500 characters")]
    public string? Bio { get; set; }

    [Required(ErrorMessage = "Location is required")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "ContactNumber is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string? ContactNumber { get; set; }
}

public class CreateUserProfileResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? FullName { get; set; }
    public string? SkillSet { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public string? ResumePath { get; set; }
    public string? LinkedInProfile { get; set; }
    public string? GitHubProfile { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
}
