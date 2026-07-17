using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the criteria submitted on the task search page.
/// </summary>
public class SearchModel
{
    /// <summary>
    /// Gets or sets the text to search for in the task title.
    /// </summary>
    [Display(Name = "Task Title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the exact creation date to filter by.
    /// </summary>
    [Display(Name = "Created Date")]
    [DataType(DataType.Date)]
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the exact due date to filter by.
    /// </summary>
    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }
}
