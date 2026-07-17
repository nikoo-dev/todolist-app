using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Manages tags in the database.
/// </summary>
public class TagDatabaseService : ITagDatabaseService
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TagDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TagModel>> GetAllTagsAsync(string userId)
    {
        var tags = await this.context.Tags
            .Where(tag => tag.Tasks.Any(t => t.AssigneeId == userId || (t.TodoList != null && t.TodoList.OwnerId == userId)))
            .OrderBy(tag => tag.Name)
            .ToListAsync();

        return tags.Select(ToApiModel);
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>> GetTasksByTagAsync(int tagId, string userId, int pageNumber, int pageSize)
    {
        var query = this.context.TodoTasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => t.Tags.Any(tag => tag.Id == tagId) && (t.AssigneeId == userId || (t.TodoList != null && t.TodoList.OwnerId == userId)))
            .OrderBy(t => t.DueDate);

        var totalCount = await query.CountAsync();
        var entities = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TodoTask>
        {
            Items = entities.Select(ToTaskModel).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TagModel>?> GetTagsForTaskAsync(int taskId, string userId)
    {
        var task = await this.FindAccessibleTaskQuery(userId)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        return task?.Tags.OrderBy(tag => tag.Name).Select(ToApiModel);
    }

    /// <inheritdoc/>
    public async Task<TagModel?> AddTagToTaskAsync(int taskId, string userId, string tagName)
    {
        var task = await this.context.TodoTasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null || !this.HasAccess(task, userId))
        {
            return null;
        }

        var normalizedName = tagName.Trim();
        var tag = await this.context.Tags.FirstOrDefaultAsync(t => t.Name == normalizedName);
        if (tag is null)
        {
            tag = new TagEntity { Name = normalizedName };
            this.context.Tags.Add(tag);
        }

        if (task.Tags.All(t => t.Id != tag.Id))
        {
            task.Tags.Add(tag);
            await this.context.SaveChangesAsync();
        }

        return ToApiModel(tag);
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, string userId)
    {
        var task = await this.context.TodoTasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null || !this.HasAccess(task, userId))
        {
            return false;
        }

        var tag = task.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is null)
        {
            return false;
        }

        task.Tags.Remove(tag);
        await this.context.SaveChangesAsync();

        return true;
    }

    private static TagModel ToApiModel(TagEntity entity) => new TagModel
    {
        Id = entity.Id,
        Name = entity.Name,
    };

    private static TodoTask ToTaskModel(TodoTaskEntity entity) => new TodoTask
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        CreatedDate = entity.CreatedDate,
        DueDate = entity.DueDate,
        Status = entity.Status,
        AssigneeId = entity.AssigneeId,
        TodoListId = entity.TodoListId,
        TodoListTitle = entity.TodoList != null ? entity.TodoList.Title : string.Empty,
        TagCount = entity.Tags.Count,
        CommentCount = entity.Comments.Count,
    };

    private bool HasAccess(TodoTaskEntity task, string userId) =>
        task.AssigneeId == userId || (task.TodoList != null && task.TodoList.OwnerId == userId);

    private IQueryable<TodoTaskEntity> FindAccessibleTaskQuery(string userId) =>
        this.context.TodoTasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Where(t => t.AssigneeId == userId || (t.TodoList != null && t.TodoList.OwnerId == userId));
}
