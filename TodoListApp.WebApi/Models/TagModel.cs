using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;

/// <summary>
/// Represents a tag that can be added to a to-do task.
/// </summary>
public class TagModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the tag.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the text of the tag.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
