namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents a to-do list. Used internally by the service layer to interact with the web API app.
/// </summary>
public class TodoList
{
    /// <summary>
    /// Gets or sets the unique identifier of the to-do list.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the to-do list.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the to-do list.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the number of tasks in the to-do list.
    /// </summary>
    public int TaskCount { get; set; }
}
