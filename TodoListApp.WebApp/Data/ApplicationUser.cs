using Microsoft.AspNetCore.Identity;

namespace TodoListApp.WebApp.Data;

/// <summary>
/// Represents an application user, extending the default Identity user with profile data.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the display name shown for the user across the application.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}
