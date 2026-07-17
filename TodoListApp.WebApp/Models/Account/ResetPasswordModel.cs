using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.Account;

/// <summary>
/// Represents the data submitted on the password reset page.
/// </summary>
public class ResetPasswordModel
{
    /// <summary>
    /// Gets or sets the email address of the user resetting the password.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password reset token issued to the user.
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
