using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;

/// <summary>
/// Represents the JSON contract for a to-do list returned or accepted by the web API.
/// </summary>
public class TodoListModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the to-do list.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the to-do list.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the to-do list.
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the number of tasks in the to-do list.
    /// </summary>
    public int TaskCount { get; set; }
}
