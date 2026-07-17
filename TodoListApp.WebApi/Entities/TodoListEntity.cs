namespace TodoListApp.WebApi.Entities;

/// <summary>
/// Represents a to-do list stored in the database.
/// </summary>
public class TodoListEntity
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
    /// Gets or sets the identifier of the user who owns the to-do list.
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tasks that belong to the to-do list.
    /// </summary>
    public ICollection<TodoTaskEntity> Tasks { get; } = new List<TodoTaskEntity>();
}
