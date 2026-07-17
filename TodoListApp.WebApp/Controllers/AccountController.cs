using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Models.Account;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles user registration, sign-in, sign-out, and password recovery.
/// </summary>
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly ILogger<AccountController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    /// <param name="logger">The logger.</param>
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
    }

    /// <summary>
    /// Shows the sign-up page.
    /// </summary>
    /// <returns>The sign-up view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register() => this.View(new RegisterModel());

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="model">The registration data.</param>
    /// <returns>A redirect to the to-do list index on success; otherwise, the form with validation errors.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName,
        };

        var result = await this.userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        this.logger.LogInformation("User {Email} registered a new account.", model.Email);
        await this.signInManager.SignInAsync(user, isPersistent: false);

        return this.RedirectToAction("Index", "TodoList");
    }

    /// <summary>
    /// Shows the sign-in page.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after a successful sign-in.</param>
    /// <returns>The sign-in view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null) => this.View(new LoginModel { ReturnUrl = returnUrl });

    /// <summary>
    /// Signs the user in.
    /// </summary>
    /// <param name="model">The sign-in data.</param>
    /// <returns>A redirect to the return URL on success; otherwise, the form with validation errors.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var result = await this.signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            this.ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return this.View(model);
        }

        this.logger.LogInformation("User {Email} signed in.", model.Email);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && this.Url.IsLocalUrl(model.ReturnUrl))
        {
            return this.Redirect(model.ReturnUrl);
        }

        return this.RedirectToAction("Index", "TodoList");
    }

    /// <summary>
    /// Signs the current user out.
    /// </summary>
    /// <returns>A redirect to the sign-in page.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await this.signInManager.SignOutAsync();
        this.logger.LogInformation("User signed out.");

        return this.RedirectToAction(nameof(this.Login));
    }

    /// <summary>
    /// Shows the password recovery request page.
    /// </summary>
    /// <returns>The forgot password view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword() => this.View(new ForgotPasswordModel());

    /// <summary>
    /// Sends password recovery instructions to the specified email address.
    /// </summary>
    /// <param name="model">The password recovery request data.</param>
    /// <returns>The confirmation page, showing the reset link since no mail server is configured.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = await this.userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            // Do not reveal that the user does not exist.
            return this.RedirectToAction(nameof(this.ForgotPasswordConfirmation));
        }

        var token = await this.userManager.GeneratePasswordResetTokenAsync(user);
        var resetUrl = this.Url.Action(
            nameof(this.ResetPassword),
            "Account",
            new { email = model.Email, token },
            this.Request.Scheme);

        this.logger.LogInformation("Password reset requested for {Email}. Reset link: {ResetUrl}", model.Email, resetUrl);

        this.TempData["ResetPasswordUrl"] = resetUrl;

        return this.RedirectToAction(nameof(this.ForgotPasswordConfirmation));
    }

    /// <summary>
    /// Shows the password recovery confirmation page.
    /// </summary>
    /// <returns>The confirmation view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation() => this.View();

    /// <summary>
    /// Shows the password reset page.
    /// </summary>
    /// <param name="email">The email address of the user resetting the password.</param>
    /// <param name="token">The password reset token.</param>
    /// <returns>The reset password view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string email, string token) =>
        this.View(new ResetPasswordModel { Email = email, Token = token });

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="model">The password reset data.</param>
    /// <returns>A redirect to the confirmation page on success; otherwise, the form with validation errors.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = await this.userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            // Do not reveal that the user does not exist.
            return this.RedirectToAction(nameof(this.ResetPasswordConfirmation));
        }

        var result = await this.userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        this.logger.LogInformation("Password reset for {Email}.", model.Email);

        return this.RedirectToAction(nameof(this.ResetPasswordConfirmation));
    }

    /// <summary>
    /// Shows the password reset confirmation page.
    /// </summary>
    /// <returns>The confirmation view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation() => this.View();

    /// <summary>
    /// Shows the access denied page.
    /// </summary>
    /// <returns>The access denied view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => this.View();
}
