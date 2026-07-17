using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Provides operations to manage comments in the database.
/// </summary>
public interface ICommentDatabaseService
{
    /// <summary>
    /// Gets the comments added to a task the specified user has access to.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>The comments, ordered by creation date, or <see langword="null"/> if the task does not exist or the user does not have access to it.</returns>
    Task<IEnumerable<CommentModel>?> GetCommentsForTaskAsync(int taskId, string userId);

    /// <summary>
    /// Adds a new comment to a task the specified user has access to.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the comment author.</param>
    /// <param name="text">The text of the comment.</param>
    /// <returns>The added comment, or <see langword="null"/> if the task does not exist or the user does not have access to it.</returns>
    Task<CommentModel?> AddCommentAsync(int taskId, string userId, string text);

    /// <summary>
    /// Updates an existing comment. Allowed for the to-do list owner (per US25) or the comment's own author.
    /// </summary>
    /// <param name="commentId">The identifier of the comment.</param>
    /// <param name="userId">The identifier of the current user.</param>
    /// <param name="text">The updated text of the comment.</param>
    /// <returns><see langword="true"/> if the comment was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateCommentAsync(int commentId, string userId, string text);

    /// <summary>
    /// Deletes an existing comment. Allowed for the to-do list owner (per US24) or the comment's own author.
    /// </summary>
    /// <param name="commentId">The identifier of the comment.</param>
    /// <param name="userId">The identifier of the current user.</param>
    /// <returns><see langword="true"/> if the comment was deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteCommentAsync(int commentId, string userId);
}
