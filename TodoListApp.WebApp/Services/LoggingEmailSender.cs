using Microsoft.AspNetCore.Identity.UI.Services;
using TodoListApp.WebApp.Logging;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// An <see cref="IEmailSender"/> implementation that writes emails to the application log instead of
/// sending them, since this environment does not have an SMTP server configured.
/// </summary>
public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingEmailSender"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        this.logger.EmailSuppressed(email, subject, htmlMessage);

        return Task.CompletedTask;
    }
}
