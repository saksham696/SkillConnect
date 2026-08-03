using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenHelper
{
    private readonly string _secretKey;
    private readonly int _expireMinutes;
    private readonly string _issuer;
    private readonly string _audience;
    public JwtTokenHelper(IConfiguration configuration)
    {
        _secretKey = configuration["JwtSettings:SecretKey"];
        _expireMinutes = int.Parse(configuration["JwtSettings:TokenExpirationMinutes"]);
        _issuer = configuration["JwtSettings:Issuer"];
        _audience = configuration["JwtSettings:Audience"];
    }
    public string GenerateToken(int userId, string email, string name, string type)
    {
        int expireMinutes = _expireMinutes;
        string secretKey = _secretKey;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, type)
            }),
            Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _issuer,
            Audience = _audience
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}