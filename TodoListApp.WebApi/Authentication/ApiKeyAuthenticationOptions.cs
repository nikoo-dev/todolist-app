using Microsoft.AspNetCore.Authentication;

namespace TodoListApp.WebApi.Authentication;

/// <summary>
/// Options for the API key bearer authentication scheme.
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// The name of the authentication scheme.
    /// </summary>
    public const string SchemeName = "ApiKey";

    /// <summary>
    /// Gets or sets the expected API key value.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
