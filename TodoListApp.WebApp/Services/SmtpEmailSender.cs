using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using TodoListApp.WebApp.Logging;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// An <see cref="IEmailSender"/> implementation that delivers email through a configured SMTP server.
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions options;
    private readonly ILogger<SmtpEmailSender> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
    /// </summary>
    /// <param name="options">The SMTP server settings.</param>
    /// <param name="logger">The logger.</param>
    public SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.options = options.Value;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var client = new SmtpClient(this.options.Host, this.options.Port)
        {
            EnableSsl = this.options.EnableSsl,
            Credentials = new NetworkCredential(this.options.Username, this.options.Password),
        };

        using var message = new MailMessage
        {
            From = new MailAddress(this.options.FromAddress, this.options.FromName),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };
        message.To.Add(email);

        try
        {
            await client.SendMailAsync(message);
            this.logger.EmailSent(email, subject);
        }
        catch (SmtpException ex)
        {
            this.logger.EmailSendFailed(email, subject, ex.Message);
        }
    }
}
