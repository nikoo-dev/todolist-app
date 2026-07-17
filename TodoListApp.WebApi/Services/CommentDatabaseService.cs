using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Manages comments in the database.
/// </summary>
public class CommentDatabaseService : ICommentDatabaseService
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CommentDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CommentModel>?> GetCommentsForTaskAsync(int taskId, string userId)
    {
        var task = await this.context.TodoTasks
            .AsNoTracking()
            .Include(t => t.TodoList)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null || !HasAccess(task, userId))
        {
            return null;
        }

        return task.Comments.OrderBy(c => c.CreatedDate).Select(ToApiModel);
    }

    /// <inheritdoc/>
    public async Task<CommentModel?> GetCommentAsync(int commentId, string userId)
    {
        var comment = await this.FindEditableCommentQuery(userId)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == commentId);

        return comment is null ? null : ToApiModel(comment);
    }

    /// <inheritdoc/>
    public async Task<CommentModel?> AddCommentAsync(int taskId, string userId, string text)
    {
        var task = await this.context.TodoTasks
            .Include(t => t.TodoList)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null || !HasAccess(task, userId))
        {
            return null;
        }

        var comment = new CommentEntity
        {
            Text = text,
            TaskId = taskId,
            AuthorId = userId,
            CreatedDate = DateTime.Now,
        };

        this.context.Comments.Add(comment);
        await this.context.SaveChangesAsync();

        return ToApiModel(comment);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateCommentAsync(int commentId, string userId, string text)
    {
        var comment = await this.FindEditableCommentQuery(userId)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment is null)
        {
            return false;
        }

        comment.Text = text;
        await this.context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteCommentAsync(int commentId, string userId)
    {
        var comment = await this.FindEditableCommentQuery(userId)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment is null)
        {
            return false;
        }

        this.context.Comments.Remove(comment);
        await this.context.SaveChangesAsync();

        return true;
    }

    private static bool HasAccess(TodoTaskEntity task, string userId) =>
        task.AssigneeId == userId || (task.TodoList != null && task.TodoList.OwnerId == userId);

    private static CommentModel ToApiModel(CommentEntity entity) => new CommentModel
    {
        Id = entity.Id,
        Text = entity.Text,
        TaskId = entity.TaskId,
        AuthorId = entity.AuthorId,
        CreatedDate = entity.CreatedDate,
    };

    /// <summary>
    /// Finds a comment the specified user is allowed to edit or delete: either the to-do list owner
    /// (per US24/US25) or the comment's own author.
    /// </summary>
    private IQueryable<CommentEntity> FindEditableCommentQuery(string userId) =>
        this.context.Comments
            .Include(c => c.Task)
                .ThenInclude(t => t!.TodoList)
            .Where(c => c.AuthorId == userId || (c.Task != null && c.Task.TodoList != null && c.Task.TodoList.OwnerId == userId));
}
