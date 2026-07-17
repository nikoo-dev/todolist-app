using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents a comment added to a to-do task.
/// </summary>
public class CommentModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the comment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the text of the comment.
    /// </summary>
    [Required(ErrorMessage = "Comment text is required.")]
    [MaxLength(2000)]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the task the comment belongs to.
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who authored the comment.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the user who authored the comment.
    /// </summary>
    public string AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the comment was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
