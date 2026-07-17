namespace TodoListApp.WebApp.Services;

/// <summary>
/// SMTP server settings used by <see cref="SmtpEmailSender"/>. Bound from the "Smtp" configuration
/// section, typically supplied through user-secrets or environment variables rather than
/// appsettings.json, since it can carry real mailbox credentials.
/// </summary>
public class SmtpOptions
{
    /// <summary>
    /// Gets or sets the SMTP server host name.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SMTP server port.
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Gets or sets a value indicating whether to use SSL/TLS when connecting.
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Gets or sets the SMTP authentication username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SMTP authentication password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender's email address.
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender's display name.
    /// </summary>
    public string FromName { get; set; } = "To-Do List App";
}
