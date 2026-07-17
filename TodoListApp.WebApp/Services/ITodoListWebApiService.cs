using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Provides operations to manage to-do lists using the TodoListApp.WebApi REST API. The current user's
/// identity is asserted implicitly via the JWT attached to each outgoing request.
/// </summary>
public interface ITodoListWebApiService
{
    /// <summary>
    /// Gets a page of to-do lists owned by the current user.
    /// </summary>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of to-do lists.</returns>
    Task<PagedResult<TodoList>> GetTodoListsAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Gets a single to-do list owned by the current user.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <returns>The to-do list, or <see langword="null"/> if it does not exist.</returns>
    Task<TodoList?> GetTodoListAsync(int id);

    /// <summary>
    /// Adds a new to-do list.
    /// </summary>
    /// <param name="todoList">The to-do list to add.</param>
    /// <returns>The added to-do list.</returns>
    Task<TodoList> AddTodoListAsync(TodoList todoList);

    /// <summary>
    /// Updates an existing to-do list.
    /// </summary>
    /// <param name="todoList">The to-do list with updated data.</param>
    /// <returns><see langword="true"/> if the to-do list was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateTodoListAsync(TodoList todoList);

    /// <summary>
    /// Deletes an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <returns><see langword="true"/> if the to-do list was deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteTodoListAsync(int id);
}
