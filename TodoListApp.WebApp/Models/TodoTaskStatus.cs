namespace TodoListApp.WebApp.Models;

/// <summary>
/// Describes the current state of a to-do task.
/// </summary>
public enum TodoTaskStatus
{
    /// <summary>
    /// The task has not been started yet.
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// The task is being worked on.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// The task has been completed.
    /// </summary>
    Completed = 2,
}
