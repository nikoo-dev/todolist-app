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
    public async Task<bool> UpdateCommentAsync(int commentId, string ownerId, string text)
    {
        var comment = await this.FindOwnedCommentQuery(ownerId)
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
    public async Task<bool> DeleteCommentAsync(int commentId, string ownerId)
    {
        var comment = await this.FindOwnedCommentQuery(ownerId)
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

    private IQueryable<CommentEntity> FindOwnedCommentQuery(string ownerId) =>
        this.context.Comments
            .Include(c => c.Task)
                .ThenInclude(t => t!.TodoList)
            .Where(c => c.Task != null && c.Task.TodoList != null && c.Task.TodoList.OwnerId == ownerId);
}
