using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Components;

/// <summary>
/// Renders the list of comments added to a task, along with a form to add a new comment.
/// </summary>
public class TaskCommentsViewComponent : ViewComponent
{
    private readonly ICommentWebApiService commentService;
    private readonly UserManager<ApplicationUser> userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskCommentsViewComponent"/> class.
    /// </summary>
    /// <param name="commentService">The service used to manage comments.</param>
    /// <param name="userManager">The user manager, used to resolve author display names.</param>
    public TaskCommentsViewComponent(ICommentWebApiService commentService, UserManager<ApplicationUser> userManager)
    {
        this.commentService = commentService;
        this.userManager = userManager;
    }

    /// <summary>
    /// Invokes the view component.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <returns>The rendered view component result.</returns>
    public async Task<IViewComponentResult> InvokeAsync(int taskId)
    {
        var comments = await this.commentService.GetCommentsAsync(taskId);

        var authorIds = comments.Select(c => c.AuthorId).Distinct().ToList();
        var authorNames = await this.userManager.Users
            .AsNoTracking()
            .Where(u => authorIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName);

        foreach (var comment in comments)
        {
            comment.AuthorName = authorNames.TryGetValue(comment.AuthorId, out var displayName) ? displayName : comment.AuthorId;
        }

        this.ViewBag.TaskId = taskId;

        return this.View(comments);
    }
}
