using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for authentication (register/login/logout) and job-seeker
/// profile management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserHelper _currentUserHelper;

    public UserController(IUserRepository userRepository, ICurrentUserHelper currentUserHelper)
    {
        _userRepository = userRepository;
        _currentUserHelper = currentUserHelper;
    }

    [HttpPost("create", Name = "CreateUser")]
    public async Task<IActionResult> CreateUser(CreateUserRequestDto request)
    {
        if (request.UserType != "Company" && request.UserType != "JobSeeker")
        {
            return BadRequest(new { message = "UserType must be either 'Company' or 'JobSeeker'." });
        }
        var result = await _userRepository.CreateUserAsync(request);
        return Ok(new { message = result });
    }

    [HttpPost("login", Name = "login")]
    public async Task<LoginResponseDto> Login(LoginRequestDto request)
    {
        var result = await _userRepository.LoginAsync(request);
        return await Task.FromResult(result);
    }

    /// <summary>
    /// JWTs are stateless, so the authoritative logout action is the client
    /// discarding its token. This endpoint exists so logout is still an
    /// explicit, auditable server call (and a natural place to plug in a
    /// token-blacklist / refresh-token revocation later).
    /// </summary>
    [Authorize]
    [HttpPost("logout", Name = "logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully." });
    }

    [Authorize]
    [HttpPost, Route("profile/create")]
    [Consumes("multipart/form-data")]
    public async Task<CreateUserProfileResponseDto> CreateProfile([FromForm] CreateUserProfileRequestDto request)
    {
        var result = await _userRepository.CreateUserProfileAsync(request);
        return result;
    }
}