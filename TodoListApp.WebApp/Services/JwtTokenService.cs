using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Issues short-lived JSON Web Tokens used to authenticate calls to the TodoListApp.WebApi application.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly SymmetricSecurityKey signingKey;
    private readonly string issuer;
    private readonly string audience;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public JwtTokenService(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var signingKeyValue = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");

        this.signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyValue));
        this.issuer = configuration["Jwt:Issuer"] ?? "TodoListApp";
        this.audience = configuration["Jwt:Audience"] ?? "TodoListApp.WebApi";
    }

    /// <inheritdoc/>
    public string CreateToken(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
        var credentials = new SigningCredentials(this.signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            this.issuer,
            this.audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
