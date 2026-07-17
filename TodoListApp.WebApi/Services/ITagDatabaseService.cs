using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Provides operations to manage tags in the database.
/// </summary>
public interface ITagDatabaseService
{
    /// <summary>
    /// Gets all tags added to tasks assigned to the specified user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>The tags, ordered by name.</returns>
    Task<IEnumerable<TagModel>> GetAllTagsAsync(string userId);

    /// <summary>
    /// Gets a page of tasks assigned to the specified user that are tagged with the specified tag.
    /// </summary>
    /// <param name="tagId">The identifier of the tag.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of tasks.</returns>
    Task<PagedResult<TodoTask>> GetTasksByTagAsync(int tagId, string userId, int pageNumber, int pageSize);

    /// <summary>
    /// Gets the tags added to a task the specified user has access to.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>The tags, or <see langword="null"/> if the task does not exist or the user does not have access to it.</returns>
    Task<IEnumerable<TagModel>?> GetTagsForTaskAsync(int taskId, string userId);

    /// <summary>
    /// Adds a tag to a task the specified user has access to. The tag is created if it does not already exist.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="tagName">The text of the tag.</param>
    /// <returns>The added tag, or <see langword="null"/> if the task does not exist or the user does not have access to it.</returns>
    Task<TagModel?> AddTagToTaskAsync(int taskId, string userId, string tagName);

    /// <summary>
    /// Removes a tag from a task the specified user has access to.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="tagId">The identifier of the tag.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns><see langword="true"/> if the tag was removed; otherwise, <see langword="false"/>.</returns>
    Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, string userId);
}
