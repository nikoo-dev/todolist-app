using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApi.Services;

/// <summary>
/// Provides operations to manage to-do tasks in the database.
/// </summary>
public interface ITodoTaskDatabaseService
{
    /// <summary>
    /// Gets a page of tasks that belong to the specified to-do list.
    /// </summary>
    /// <param name="todoListId">The identifier of the to-do list.</param>
    /// <param name="ownerId">The identifier of the to-do list owner.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of tasks, or <see langword="null"/> if the to-do list does not exist or is not owned by the user.</returns>
    Task<PagedResult<TodoTask>?> GetTasksAsync(int todoListId, string ownerId, int pageNumber, int pageSize);

    /// <summary>
    /// Gets a single task the specified user has access to, either because they own the containing
    /// to-do list or because the task is assigned to them.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="userId">The identifier of the current user.</param>
    /// <returns>The task, or <see langword="null"/> if it does not exist or the user does not have access to it.</returns>
    Task<TodoTask?> GetTaskAsync(int id, string userId);

    /// <summary>
    /// Adds a new task to a to-do list owned by the specified user.
    /// </summary>
    /// <param name="task">The task to add.</param>
    /// <param name="ownerId">The identifier of the to-do list owner.</param>
    /// <returns>The added task, or <see langword="null"/> if the target to-do list does not exist or is not owned by the user.</returns>
    Task<TodoTask?> AddTaskAsync(TodoTask task, string ownerId);

    /// <summary>
    /// Updates an existing task that belongs to a to-do list owned by the specified user, including
    /// reassigning it to a different user when <see cref="TodoTask.AssigneeId"/> is set.
    /// </summary>
    /// <param name="task">The task with updated data.</param>
    /// <param name="ownerId">The identifier of the to-do list owner.</param>
    /// <returns><see langword="true"/> if the task was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateTaskAsync(TodoTask task, string ownerId);

    /// <summary>
    /// Deletes an existing task that belongs to a to-do list owned by the specified user.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="ownerId">The identifier of the to-do list owner.</param>
    /// <returns><see langword="true"/> if the task was deleted; otherwise, <see langword="false"/>.</returns>
    Task<bool> DeleteTaskAsync(int id, string ownerId);

    /// <summary>
    /// Gets a page of tasks assigned to the specified user.
    /// </summary>
    /// <param name="assigneeId">The identifier of the assignee.</param>
    /// <param name="statusFilter">
    /// The status to filter by. When <see langword="null"/> and <paramref name="showAll"/> is <see langword="false"/>,
    /// only active tasks (not started or in progress) are returned.
    /// </param>
    /// <param name="showAll">When <see langword="true"/>, tasks of every status are returned, ignoring <paramref name="statusFilter"/>.</param>
    /// <param name="sortBy">The field to sort by: <c>"title"</c> or <c>"dueDate"</c> (the default).</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of assigned tasks.</returns>
    Task<PagedResult<TodoTask>> GetAssignedTasksAsync(
        string assigneeId,
        TodoTaskStatus? statusFilter,
        bool showAll,
        string? sortBy,
        int pageNumber,
        int pageSize);

    /// <summary>
    /// Updates the status of a task assigned to the specified user.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="assigneeId">The identifier of the assignee.</param>
    /// <param name="status">The new status.</param>
    /// <returns><see langword="true"/> if the status was updated; otherwise, <see langword="false"/>.</returns>
    Task<bool> UpdateTaskStatusAsync(int id, string assigneeId, TodoTaskStatus status);

    /// <summary>
    /// Searches the tasks assigned to the specified user by title, creation date, or due date.
    /// </summary>
    /// <param name="assigneeId">The identifier of the assignee.</param>
    /// <param name="title">The text to search for in the task title, or <see langword="null"/> to not filter by title.</param>
    /// <param name="createdDate">The exact creation date to filter by, or <see langword="null"/> to not filter by creation date.</param>
    /// <param name="dueDate">The exact due date to filter by, or <see langword="null"/> to not filter by due date.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of matching tasks.</returns>
    Task<PagedResult<TodoTask>> SearchTasksAsync(
        string assigneeId,
        string? title,
        DateTime? createdDate,
        DateTime? dueDate,
        int pageNumber,
        int pageSize);
}
