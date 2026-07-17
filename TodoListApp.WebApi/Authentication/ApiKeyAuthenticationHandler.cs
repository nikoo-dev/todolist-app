using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TodoListApp.WebApi.Authentication;

/// <summary>
/// Authenticates requests by validating a static API key sent as a bearer token.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">The scheme options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <inheritdoc/>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!this.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header."));
        }

        var headerValue = authorizationHeader.ToString();
        const string prefix = "Bearer ";
        if (!headerValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization scheme."));
        }

        var token = headerValue[prefix.Length..].Trim();
        if (string.IsNullOrEmpty(this.Options.ApiKey) || token != this.Options.ApiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "TrustedClient") };
        var identity = new ClaimsIdentity(claims, this.Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
