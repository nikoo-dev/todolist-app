using Microsoft.Extensions.Logging;

namespace TodoListApp.WebApp.Logging;

/// <summary>
/// Source-generated, allocation-free log message definitions used across the application.
/// </summary>
internal static partial class LogMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "User {Email} registered a new account.")]
    public static partial void UserRegistered(this ILogger logger, string email);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "User {Email} signed in.")]
    public static partial void UserSignedIn(this ILogger logger, string email);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "User signed out.")]
    public static partial void UserSignedOut(this ILogger logger);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Password reset requested for {Email}. Reset link: {ResetUrl}")]
    public static partial void PasswordResetRequested(this ILogger logger, string email, string? resetUrl);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Password reset for {Email}.")]
    public static partial void PasswordReset(this ILogger logger, string email);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Task {TaskId} deleted.")]
    public static partial void TaskDeleted(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "To-do list {TodoListId} deleted.")]
    public static partial void TodoListDeleted(this ILogger logger, int todoListId);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information, Message = "Email suppressed (no SMTP configured). To: {Email}, Subject: {Subject}, Body: {Body}")]
    public static partial void EmailSuppressed(this ILogger logger, string email, string subject, string body);

    [LoggerMessage(EventId = 9, Level = LogLevel.Information, Message = "Email sent to {Email} with subject '{Subject}'.")]
    public static partial void EmailSent(this ILogger logger, string email, string subject);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Failed to send email to {Email} with subject '{Subject}': {Error}")]
    public static partial void EmailSendFailed(this ILogger logger, string email, string subject, string error);
}
