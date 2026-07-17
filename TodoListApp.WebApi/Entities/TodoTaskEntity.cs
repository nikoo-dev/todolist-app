namespace TodoListApp.WebApi.Entities;

/// <summary>
/// Represents a to-do task stored in the database.
/// </summary>
public class TodoTaskEntity
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
    /// Gets or sets the to-do list the task belongs to.
    /// </summary>
    public TodoListEntity? TodoList { get; set; }

    /// <summary>
    /// Gets or sets the tags added to the task.
    /// </summary>
    public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();

    /// <summary>
    /// Gets or sets the comments added to the task.
    /// </summary>
    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
}
