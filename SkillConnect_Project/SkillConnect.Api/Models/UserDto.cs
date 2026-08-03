using System.ComponentModel.DataAnnotations;

public class CreateUserRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "UserType is required")]
    public string UserType { get; set; }
}

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}
