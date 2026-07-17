using System.Net.Http.Headers;
using System.Security.Claims;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Attaches a short-lived JWT identifying the current signed-in user to every outgoing request to the
/// TodoListApp.WebApi application, replacing the need for the caller to pass a client-supplied user
/// identifier that the API would otherwise have to trust unverified.
/// </summary>
public class ApiAuthDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IJwtTokenService tokenService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiAuthDelegatingHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">Provides access to the current HTTP context.</param>
    /// <param name="tokenService">The service used to issue JWTs.</param>
    public ApiAuthDelegatingHandler(IHttpContextAccessor httpContextAccessor, IJwtTokenService tokenService)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.tokenService = tokenService;
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userId = this.httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            var token = this.tokenService.CreateToken(userId);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
