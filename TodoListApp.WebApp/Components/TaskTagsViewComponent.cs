using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Components;

/// <summary>
/// Renders the list of tags added to a task, along with a form to add a new tag.
/// </summary>
public class TaskTagsViewComponent : ViewComponent
{
    private readonly ITagWebApiService tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskTagsViewComponent"/> class.
    /// </summary>
    /// <param name="tagService">The service used to manage tags.</param>
    public TaskTagsViewComponent(ITagWebApiService tagService)
    {
        this.tagService = tagService;
    }

    /// <summary>
    /// Invokes the view component.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <returns>The rendered view component result.</returns>
    public async Task<IViewComponentResult> InvokeAsync(int taskId)
    {
        var userId = this.UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("The current user identifier could not be resolved.");

        var tags = await this.tagService.GetTagsForTaskAsync(taskId, userId);
        this.ViewBag.TaskId = taskId;

        return this.View(tags);
    }
}
