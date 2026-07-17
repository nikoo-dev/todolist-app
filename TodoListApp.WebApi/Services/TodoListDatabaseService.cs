using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Manages to-do lists in the database.
/// </summary>
public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDatabaseService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TodoListDatabaseService(TodoListDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoList>> GetTodoListsAsync(string ownerId, int pageNumber, int pageSize)
    {
        var query = this.context.TodoLists
            .Where(l => l.OwnerId == ownerId)
            .OrderBy(l => l.Title);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new TodoList
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                OwnerId = l.OwnerId,
                TaskCount = l.Tasks.Count,
            })
            .ToListAsync();

        return new PagedResult<TodoList>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    /// <inheritdoc/>
    public async Task<TodoList?> GetTodoListAsync(int id, string ownerId)
    {
        var entity = await this.context.TodoLists
            .Include(l => l.Tasks)
            .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == ownerId);

        return entity is null ? null : ToModel(entity);
    }

    /// <inheritdoc/>
    public async Task<TodoList> AddTodoListAsync(TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        var entity = new TodoListEntity
        {
            Title = todoList.Title,
            Description = todoList.Description,
            OwnerId = todoList.OwnerId,
        };

        this.context.TodoLists.Add(entity);
        await this.context.SaveChangesAsync();

        return ToModel(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateTodoListAsync(TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        var entity = await this.context.TodoLists
            .FirstOrDefaultAsync(l => l.Id == todoList.Id && l.OwnerId == todoList.OwnerId);

        if (entity is null)
        {
            return false;
        }

        entity.Title = todoList.Title;
        entity.Description = todoList.Description;
        await this.context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTodoListAsync(int id, string ownerId)
    {
        var entity = await this.context.TodoLists
            .FirstOrDefaultAsync(l => l.Id == id && l.OwnerId == ownerId);

        if (entity is null)
        {
            return false;
        }

        this.context.TodoLists.Remove(entity);
        await this.context.SaveChangesAsync();

        return true;
    }

    private static TodoList ToModel(TodoListEntity entity) => new TodoList
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        OwnerId = entity.OwnerId,
        TaskCount = entity.Tasks.Count,
    };
}
