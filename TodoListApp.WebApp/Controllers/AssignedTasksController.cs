using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser requests for the list of tasks assigned to the current user.
/// </summary>
[Authorize]
public class AssignedTasksController : Controller
{
    private const int PageSize = 20;

    private readonly ITodoTaskWebApiService taskService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignedTasksController"/> class.
    /// </summary>
    /// <param name="taskService">The service used to manage tasks.</param>
    public AssignedTasksController(ITodoTaskWebApiService taskService)
    {
        this.taskService = taskService;
    }

    /// <summary>
    /// Shows the list of tasks assigned to the current user.
    /// </summary>
    /// <param name="statusFilter">The status to filter by: "Active" (default), "All", "NotStarted", "InProgress", or "Completed".</param>
    /// <param name="sortBy">The field to sort by: "dueDate" (default) or "title".</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <returns>The assigned task list view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(string statusFilter = "Active", string sortBy = "dueDate", int pageNumber = 1)
    {
        var showAll = string.Equals(statusFilter, "All", StringComparison.OrdinalIgnoreCase);
        TodoTaskStatus? status = Enum.TryParse<TodoTaskStatus>(statusFilter, true, out var parsed) ? parsed : null;

        var page = await this.taskService.GetAssignedTasksAsync(status, showAll, sortBy, pageNumber, PageSize);

        this.ViewBag.StatusFilter = statusFilter;
        this.ViewBag.SortBy = sortBy;

        return this.View(page);
    }

    /// <summary>
    /// Changes the status of an assigned task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="status">The new status.</param>
    /// <param name="statusFilter">The current status filter, preserved on redirect.</param>
    /// <param name="sortBy">The current sort field, preserved on redirect.</param>
    /// <param name="pageNumber">The current page number, preserved on redirect.</param>
    /// <returns>A redirect back to the assigned task list.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(
        int id,
        TodoTaskStatus status,
        string statusFilter = "Active",
        string sortBy = "dueDate",
        int pageNumber = 1)
    {
        await this.taskService.UpdateTaskStatusAsync(id, status);

        return this.RedirectToAction(nameof(this.Index), new { statusFilter, sortBy, pageNumber });
    }
}
