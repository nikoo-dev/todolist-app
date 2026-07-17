namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the JSON contract exchanged with the TodoListApp.WebApi application for a to-do list.
/// </summary>
public class TodoListWebApiModel
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
