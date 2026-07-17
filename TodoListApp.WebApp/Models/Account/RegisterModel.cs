using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models.Account;

/// <summary>
/// Represents the data submitted on the sign-up (register) page.
/// </summary>
public class RegisterModel
{
    /// <summary>
    /// Gets or sets the display name of the new user.
    /// </summary>
    [Required(ErrorMessage = "Display name is required.")]
    [StringLength(100, ErrorMessage = "Display name must be at most 100 characters long.")]
    [Display(Name = "Display Name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the new user.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password of the new user.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
