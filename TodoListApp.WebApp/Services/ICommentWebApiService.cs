using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Provides operations to manage comments using the TodoListApp.WebApi REST API. The current user's
/// identity is asserted implicitly via the JWT attached to each outgoing request.
/// </summary>
public interface ICommentWebApiService
{
    /// <summary>
    /// Gets the comments added to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <returns>The comments.</returns>
    Task<IReadOnlyList<CommentModel>> GetCommentsAsync(int taskId);

    /// <summary>
    /// Adds a new comment to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="text">The text of the comment.</param>
    /// <returns>The added comment.</returns>
    Task<CommentModel> AddCommentAsync(int taskId, string text);

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="commentId">The identifier of the comment.</param>
    /// <param name="text">The updated text of the comment.</param>
    /// <returns><see langword="true"/> if the comment was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateCommentAsync(int commentId, string text);

    /// <summary>
    /// Deletes an existing comment.
    /// </summary>
    /// <param name="commentId">The identifier of the comment.</param>
    /// <returns><see langword="true"/> if the comment was deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteCommentAsync(int commentId);
}
