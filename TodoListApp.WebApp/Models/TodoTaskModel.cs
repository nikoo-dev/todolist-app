using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the view model used to display and edit a to-do task in the browser UI.
/// </summary>
public class TodoTaskModel
{
    /// <summary>
    /// Gets or sets the unique identifier of the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must be at most 200 characters long.")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    [StringLength(2000, ErrorMessage = "Description must be at most 2000 characters long.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time the task was created.
    /// </summary>
    [Display(Name = "Created")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time the task is due.
    /// </summary>
    [Required(ErrorMessage = "Due date is required.")]
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);

    /// <summary>
    /// Gets or sets the status of the task.
    /// </summary>
    [Display(Name = "Status")]
    public TodoTaskStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the to-do list the task belongs to.
    /// </summary>
    public int TodoListId { get; set; }

    /// <summary>
    /// Gets or sets the title of the to-do list the task belongs to.
    /// </summary>
    public string TodoListTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user the task is assigned to.
    /// </summary>
    [Display(Name = "Assigned To")]
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the user the task is assigned to.
    /// </summary>
    public string AssigneeName { get; set; } = string.Empty;

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
