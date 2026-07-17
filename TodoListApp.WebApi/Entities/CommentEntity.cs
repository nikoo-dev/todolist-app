namespace TodoListApp.WebApi.Entities;

/// <summary>
/// Represents a comment added to a to-do task.
/// </summary>
public class CommentEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the comment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the text of the comment.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the task the comment belongs to.
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// Gets or sets the task the comment belongs to.
    /// </summary>
    public TodoTaskEntity? Task { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who authored the comment.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the comment was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
