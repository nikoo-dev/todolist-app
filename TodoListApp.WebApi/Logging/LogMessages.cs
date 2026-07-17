using Microsoft.Extensions.Logging;

namespace TodoListApp.WebApi.Logging;

/// <summary>
/// Source-generated, allocation-free log message definitions used across the application.
/// </summary>
internal static partial class LogMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "To-do list {TodoListId} created by user {UserId}.")]
    public static partial void TodoListCreated(this ILogger logger, int todoListId, string userId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "To-do list {TodoListId} deleted by user {UserId}.")]
    public static partial void TodoListDeleted(this ILogger logger, int todoListId, string userId);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Task {TaskId} created in to-do list {TodoListId} by user {UserId}.")]
    public static partial void TaskCreated(this ILogger logger, int taskId, int todoListId, string userId);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Task {TaskId} deleted by user {UserId}.")]
    public static partial void TaskDeleted(this ILogger logger, int taskId, string userId);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Unhandled exception while processing {Method} {Path}.")]
    public static partial void UnhandledException(this ILogger logger, Exception exception, string method, string path);
}
