using System.Security.Claims;

public interface ICurrentUserHelper
{
    int userId { get; }
}

public class CurrentUserHelper : ICurrentUserHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public int userId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // or throw an exception if you prefer
        }
    }
}