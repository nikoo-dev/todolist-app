using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Manages to-do tasks in the database.
/// </summary>
public class TodoTaskDatabaseService : ITodoTaskDatabaseService
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TodoTaskDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>?> GetTasksAsync(int todoListId, string ownerId, int pageNumber, int pageSize)
    {
        var listExists = await this.context.TodoLists.AnyAsync(l => l.Id == todoListId && l.OwnerId == ownerId);
        if (!listExists)
        {
            return null;
        }

        var query = this.context.TodoTasks
            .Where(t => t.TodoListId == todoListId)
            .OrderBy(t => t.DueDate);

        var totalCount = await query.CountAsync();
        var entities = await query
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        var items = entities.Select(ToModel).ToList();

        return new PagedResult<TodoTask>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    /// <inheritdoc/>
    public async Task<TodoTask?> GetTaskAsync(int id, string ownerId)
    {
        var entity = await this.FindOwnedTaskQuery(ownerId)
            .FirstOrDefaultAsync(t => t.Id == id);

        return entity is null ? null : ToModel(entity);
    }

    /// <inheritdoc/>
    public async Task<TodoTask?> AddTaskAsync(TodoTask task, string ownerId)
    {
        ArgumentNullException.ThrowIfNull(task);

        var listExists = await this.context.TodoLists.AnyAsync(l => l.Id == task.TodoListId && l.OwnerId == ownerId);
        if (!listExists)
        {
            return null;
        }

        var entity = new TodoTaskEntity
        {
            Title = task.Title,
            Description = task.Description,
            CreatedDate = DateTime.Now,
            DueDate = task.DueDate,
            Status = task.Status,
            AssigneeId = ownerId,
            TodoListId = task.TodoListId,
        };

        this.context.TodoTasks.Add(entity);
        await this.context.SaveChangesAsync();

        return ToModel(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateTaskAsync(TodoTask task, string ownerId)
    {
        ArgumentNullException.ThrowIfNull(task);

        var entity = await this.FindOwnedTaskQuery(ownerId)
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        if (entity is null)
        {
            return false;
        }

        entity.Title = task.Title;
        entity.Description = task.Description;
        entity.DueDate = task.DueDate;
        entity.Status = task.Status;
        await this.context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTaskAsync(int id, string ownerId)
    {
        var entity = await this.FindOwnedTaskQuery(ownerId)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (entity is null)
        {
            return false;
        }

        this.context.TodoTasks.Remove(entity);
        await this.context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>> GetAssignedTasksAsync(
        string assigneeId,
        TodoTaskStatus? statusFilter,
        bool showAll,
        string? sortBy,
        int pageNumber,
        int pageSize)
    {
        var query = this.context.TodoTasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => t.AssigneeId == assigneeId);

        if (!showAll)
        {
            query = statusFilter.HasValue
                ? query.Where(t => t.Status == statusFilter.Value)
                : query.Where(t => t.Status != TodoTaskStatus.Completed);
        }

        query = string.Equals(sortBy, "title", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(t => t.Title)
            : query.OrderBy(t => t.DueDate);

        var totalCount = await query.CountAsync();
        var entities = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TodoTask>
        {
            Items = entities.Select(ToModel).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateTaskStatusAsync(int id, string assigneeId, TodoTaskStatus status)
    {
        var entity = await this.context.TodoTasks
            .FirstOrDefaultAsync(t => t.Id == id && t.AssigneeId == assigneeId);

        if (entity is null)
        {
            return false;
        }

        entity.Status = status;
        await this.context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>> SearchTasksAsync(
        string assigneeId,
        string? title,
        DateTime? createdDate,
        DateTime? dueDate,
        int pageNumber,
        int pageSize)
    {
        var query = this.context.TodoTasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => t.AssigneeId == assigneeId);

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(t => t.Title.Contains(title));
        }

        if (createdDate.HasValue)
        {
            query = query.Where(t => t.CreatedDate.Date == createdDate.Value.Date);
        }

        if (dueDate.HasValue)
        {
            query = query.Where(t => t.DueDate.Date == dueDate.Value.Date);
        }

        query = query.OrderBy(t => t.DueDate);

        var totalCount = await query.CountAsync();
        var entities = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TodoTask>
        {
            Items = entities.Select(ToModel).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    private static TodoTask ToModel(TodoTaskEntity entity) => new TodoTask
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

    private IQueryable<TodoTaskEntity> FindOwnedTaskQuery(string ownerId) =>
        this.context.TodoTasks
            .Include(t => t.TodoList)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => t.TodoList != null && t.TodoList.OwnerId == ownerId);
}
