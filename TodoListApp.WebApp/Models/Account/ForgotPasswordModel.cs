using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.Account;

/// <summary>
/// Represents the data submitted on the password recovery request page.
/// </summary>
public class ForgotPasswordModel
{
    /// <summary>
    /// Gets or sets the email address to send password recovery instructions to.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}
