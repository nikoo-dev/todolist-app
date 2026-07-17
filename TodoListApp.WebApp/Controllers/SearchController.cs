using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser requests to search for tasks.
/// </summary>
[Authorize]
public class SearchController : Controller
{
    private const int PageSize = 20;

    private readonly ITodoTaskWebApiService taskService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// </summary>
    /// <param name="taskService">The service used to manage tasks.</param>
    public SearchController(ITodoTaskWebApiService taskService)
    {
        this.taskService = taskService;
    }

    /// <summary>
    /// Shows the search page.
    /// </summary>
    /// <returns>The search form view.</returns>
    [HttpGet]
    public IActionResult Index() => this.View(new SearchModel());

    /// <summary>
    /// Shows the search results page.
    /// </summary>
    /// <param name="title">The text to search for in the task title.</param>
    /// <param name="createdDate">The exact creation date to filter by.</param>
    /// <param name="dueDate">The exact due date to filter by.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <returns>The search results view.</returns>
    [HttpGet]
    public async Task<IActionResult> Results(string? title, DateTime? createdDate, DateTime? dueDate, int pageNumber = 1)
    {
        var page = await this.taskService.SearchTasksAsync(this.CurrentUserId, title, createdDate, dueDate, pageNumber, PageSize);

        this.ViewBag.Title = title;
        this.ViewBag.CreatedDate = createdDate;
        this.ViewBag.DueDate = dueDate;

        return this.View(page);
    }

    private string CurrentUserId =>
        this.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("The current user identifier could not be resolved.");
}
