using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Provides operations to manage tags using the TodoListApp.WebApi REST API.
/// </summary>
public interface ITagWebApiService
{
    /// <summary>
    /// Gets all tags added to tasks the current user has access to.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>The tags.</returns>
    Task<IReadOnlyList<TagModel>> GetAllTagsAsync(string userId);

    /// <summary>
    /// Gets a page of tasks tagged with the specified tag.
    /// </summary>
    /// <param name="tagId">The identifier of the tag.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of tasks.</returns>
    Task<PagedResult<TodoTask>> GetTasksByTagAsync(int tagId, string userId, int pageNumber, int pageSize);

    /// <summary>
    /// Gets the tags added to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>The tags.</returns>
    Task<IReadOnlyList<TagModel>> GetTagsForTaskAsync(int taskId, string userId);

    /// <summary>
    /// Adds a tag to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="tagName">The text of the tag.</param>
    /// <returns>The added tag.</returns>
    Task<TagModel> AddTagToTaskAsync(int taskId, string userId, string tagName);

    /// <summary>
    /// Removes a tag from a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="tagId">The identifier of the tag.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns><see langword="true"/> if the tag was removed; otherwise, <see langword="false"/>.</returns>
    Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, string userId);
}
