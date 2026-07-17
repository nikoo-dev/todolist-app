using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Provides operations to manage to-do tasks using the TodoListApp.WebApi REST API. The current user's
/// identity is asserted implicitly via the JWT attached to each outgoing request.
/// </summary>
public interface ITodoTaskWebApiService
{
    /// <summary>
    /// Gets a page of tasks that belong to the specified to-do list, owned by the current user.
    /// </summary>
    /// <param name="todoListId">The identifier of the to-do list.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of tasks, or <see langword="null"/> if the to-do list does not exist.</returns>
    Task<PagedResult<TodoTask>?> GetTasksAsync(int todoListId, int pageNumber, int pageSize);

    /// <summary>
    /// Gets a single task the current user has access to.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <returns>The task, or <see langword="null"/> if it does not exist.</returns>
    Task<TodoTask?> GetTaskAsync(int id);

    /// <summary>
    /// Adds a new task to the specified to-do list, owned by the current user.
    /// </summary>
    /// <param name="todoListId">The identifier of the to-do list.</param>
    /// <param name="task">The task to add.</param>
    /// <returns>The added task, or <see langword="null"/> if the target to-do list does not exist.</returns>
    Task<TodoTask?> AddTaskAsync(int todoListId, TodoTask task);

    /// <summary>
    /// Updates an existing task owned by the current user.
    /// </summary>
    /// <param name="task">The task with updated data.</param>
    /// <returns><see langword="true"/> if the task was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateTaskAsync(TodoTask task);

    /// <summary>
    /// Deletes an existing task owned by the current user.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <returns><see langword="true"/> if the task was deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteTaskAsync(int id);

    /// <summary>
    /// Gets a page of tasks assigned to the current user.
    /// </summary>
    /// <param name="statusFilter">The status to filter by. When <see langword="null"/> and <paramref name="showAll"/> is <see langword="false"/>, only active tasks are returned.</param>
    /// <param name="showAll">When <see langword="true"/>, tasks of every status are returned.</param>
    /// <param name="sortBy">The field to sort by: <c>"title"</c> or <c>"dueDate"</c>.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of assigned tasks.</returns>
    Task<PagedResult<TodoTask>> GetAssignedTasksAsync(
        TodoTaskStatus? statusFilter,
        bool showAll,
        string? sortBy,
        int pageNumber,
        int pageSize);

    /// <summary>
    /// Updates the status of a task assigned to the current user.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="status">The new status.</param>
    /// <returns><see langword="true"/> if the status was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateTaskStatusAsync(int id, TodoTaskStatus status);

    /// <summary>
    /// Searches the tasks assigned to the current user by title, creation date, or due date.
    /// </summary>
    /// <param name="title">The text to search for in the task title.</param>
    /// <param name="createdDate">The exact creation date to filter by.</param>
    /// <param name="dueDate">The exact due date to filter by.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of matching tasks.</returns>
    Task<PagedResult<TodoTask>> SearchTasksAsync(
        string? title,
        DateTime? createdDate,
        DateTime? dueDate,
        int pageNumber,
        int pageSize);
}
