using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.Account;

/// <summary>
/// Represents the data submitted on the sign-in (login) page.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the sign-in cookie should persist across browser sessions.
    /// </summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    /// <summary>
    /// Gets or sets the URL to return to after a successful sign-in.
    /// </summary>
    public string? ReturnUrl { get; set; }
}
