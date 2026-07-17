using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Provides operations to manage to-do lists using the TodoListApp.WebApi REST API.
/// </summary>
public interface ITodoListWebApiService
{
    /// <summary>
    /// Gets a page of to-do lists owned by the specified user.
    /// </summary>
    /// <param name="userId">The identifier of the owner.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of to-do lists.</returns>
    Task<PagedResult<TodoList>> GetTodoListsAsync(string userId, int pageNumber, int pageSize);

    /// <summary>
    /// Gets a single to-do list owned by the specified user.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <param name="userId">The identifier of the owner.</param>
    /// <returns>The to-do list, or <see langword="null"/> if it does not exist.</returns>
    Task<TodoList?> GetTodoListAsync(int id, string userId);

    /// <summary>
    /// Adds a new to-do list.
    /// </summary>
    /// <param name="userId">The identifier of the owner.</param>
    /// <param name="todoList">The to-do list to add.</param>
    /// <returns>The added to-do list.</returns>
    Task<TodoList> AddTodoListAsync(string userId, TodoList todoList);

    /// <summary>
    /// Updates an existing to-do list.
    /// </summary>
    /// <param name="userId">The identifier of the owner.</param>
    /// <param name="todoList">The to-do list with updated data.</param>
    /// <returns><see langword="true"/> if the to-do list was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateTodoListAsync(string userId, TodoList todoList);

    /// <summary>
    /// Deletes an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <param name="userId">The identifier of the owner.</param>
    /// <returns><see langword="true"/> if the to-do list was deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteTodoListAsync(int id, string userId);
}
