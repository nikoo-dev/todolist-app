namespace TodoListApp.WebApi.Entities;

/// <summary>
/// Represents a tag that can be added to a to-do task.
/// </summary>
public class TagEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the tag.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the text of the tag.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tasks that are tagged with this tag.
    /// </summary>
    public ICollection<TodoTaskEntity> Tasks { get; set; } = new List<TodoTaskEntity>();
}
