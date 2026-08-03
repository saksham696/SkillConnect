using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly SkillConnectDbContext _dbContext;
    private readonly JwtTokenHelper _jwtTokenHelper;
    private readonly IFileUploadHelper _fileUploadHelper;
    private readonly ICurrentUserHelper _currentUserHelper;
    public UserRepository(SkillConnectDbContext dbContext, JwtTokenHelper jwtTokenHelper,
    IFileUploadHelper fileUploadHelper, ICurrentUserHelper currentUserHelper)
    {
        _dbContext = dbContext;
        _jwtTokenHelper = jwtTokenHelper;
        _fileUploadHelper = fileUploadHelper;
        _currentUserHelper = currentUserHelper;
    }
    public Task<string> CreateUserAsync(CreateUserRequestDto request)
    {
        User newUser = new User();
        newUser.Name = request.Name;
        newUser.Email = request.Email;
        newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);// Use BCrypt
        newUser.Type = request.UserType;
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.IsActive = true;

        _dbContext.Users.Add(newUser);
        var result = _dbContext.SaveChanges();
        if (result > 0)
        {
            return Task.FromResult("User created successfully");
        }
        else
        {
            return Task.FromResult("Failed to create user");
        }
    }
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResponseDto(); // Invalid credentials
        }
        var token = _jwtTokenHelper.GenerateToken(user.Id, user.Email, user.Name, user.Type);
        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Type = user.Type
        };
    }
    public Task<CreateUserProfileResponseDto> CreateUserProfileAsync(CreateUserProfileRequestDto request)
    {
        string resumePath = string.Empty;
        if (request.ResumeFile != null)
        {
            resumePath = _fileUploadHelper.UploadResumeAsync(request.ResumeFile).Result;
        }

        UserProfile profile = new UserProfile
        {
            UserId = _currentUserHelper.userId,
            FullName = request.FullName,
            Bio = request.Bio,
            SkillSet = request.SkillSet,
            Experience = request.Experience,
            Education = request.Education,
            Location = request.Location,
            ResumePath = resumePath, // Should be fetched from the FileUploadHelper after uploading the file
            ContactNumber = request.ContactNumber,
            LinkedInProfile = request.LinkedInProfile,
            GitHubProfile = request.GitHubProfile
        };
        _dbContext.UserProfiles.Add(profile);
        var result = _dbContext.SaveChanges();
        if (result > 0)
        {
            return Task.FromResult(new
             CreateUserProfileResponseDto
            {
                Message = "User profile created successfully!",
                Success = true
            }
             );
        }
        else
        {
            return Task.FromResult(new
            CreateUserProfileResponseDto
            {
                Message = "Failed to create user profile.",
                Success = false
            }
            );
        }
    }

}