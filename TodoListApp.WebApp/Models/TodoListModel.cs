using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the view model used to display and edit a to-do list in the browser UI.
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
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must be at most 200 characters long.")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the to-do list.
    /// </summary>
    [StringLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the number of tasks in the to-do list.
    /// </summary>
    [Display(Name = "Tasks")]
    public int TaskCount { get; set; }
}
