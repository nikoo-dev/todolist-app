namespace TodoListApp.WebApp.Services;

/// <summary>
/// Issues short-lived JSON Web Tokens used to authenticate calls to the TodoListApp.WebApi application.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Creates a signed JWT asserting the specified user's identity.
    /// </summary>
    /// <param name="userId">The identifier of the current user.</param>
    /// <returns>The encoded JWT.</returns>
    string CreateToken(string userId);
}
