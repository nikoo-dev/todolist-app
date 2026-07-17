using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser requests to view tags.
/// </summary>
[Authorize]
public class TagController : Controller
{
    private const int PageSize = 20;

    private readonly ITagWebApiService tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagController"/> class.
    /// </summary>
    /// <param name="tagService">The service used to manage tags.</param>
    public TagController(ITagWebApiService tagService)
    {
        this.tagService = tagService;
    }

    private string CurrentUserId =>
        this.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("The current user identifier could not be resolved.");

    /// <summary>
    /// Shows the list of all tags.
    /// </summary>
    /// <returns>The tag list view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tags = await this.tagService.GetAllTagsAsync(this.CurrentUserId);

        return this.View(tags);
    }

    /// <summary>
    /// Shows the list of tasks tagged with the specified tag.
    /// </summary>
    /// <param name="id">The identifier of the tag.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <returns>The tasks-by-tag view.</returns>
    [HttpGet]
    public async Task<IActionResult> Tasks(int id, int pageNumber = 1)
    {
        var page = await this.tagService.GetTasksByTagAsync(id, this.CurrentUserId, pageNumber, PageSize);
        this.ViewBag.TagId = id;

        return this.View(page);
    }

    /// <summary>
    /// Adds a tag to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="tagName">The text of the tag.</param>
    /// <returns>A redirect back to the task details page.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToTask(int taskId, string tagName)
    {
        if (!string.IsNullOrWhiteSpace(tagName))
        {
            await this.tagService.AddTagToTaskAsync(taskId, this.CurrentUserId, tagName);
        }

        return this.RedirectToAction("Details", "TodoTask", new { id = taskId });
    }

    /// <summary>
    /// Removes a tag from a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="tagId">The identifier of the tag.</param>
    /// <returns>A redirect back to the task details page.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromTask(int taskId, int tagId)
    {
        await this.tagService.RemoveTagFromTaskAsync(taskId, tagId, this.CurrentUserId);

        return this.RedirectToAction("Details", "TodoTask", new { id = taskId });
    }
}
