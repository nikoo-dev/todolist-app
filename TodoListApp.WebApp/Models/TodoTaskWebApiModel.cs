namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the JSON contract exchanged with the TodoListApp.WebApi application for a to-do task.
/// </summary>
public class TodoTaskWebApiModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time the task was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time the task is due.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets the status of the task.
    /// </summary>
    public TodoTaskStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user the task is assigned to.
    /// </summary>
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the to-do list the task belongs to.
    /// </summary>
    public int TodoListId { get; set; }

    /// <summary>
    /// Gets or sets the title of the to-do list the task belongs to.
    /// </summary>
    public string TodoListTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of tags added to the task.
    /// </summary>
    public int TagCount { get; set; }

    /// <summary>
    /// Gets or sets the number of comments added to the task.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the task is overdue.
    /// </summary>
    public bool IsOverdue { get; set; }
}
