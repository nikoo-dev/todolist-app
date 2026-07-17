using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser requests to manage comments on to-do tasks.
/// </summary>
[Authorize]
public class CommentController : Controller
{
    private readonly ICommentWebApiService commentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentController"/> class.
    /// </summary>
    /// <param name="commentService">The service used to manage comments.</param>
    public CommentController(ICommentWebApiService commentService)
    {
        this.commentService = commentService;
    }

    /// <summary>
    /// Adds a new comment to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="text">The text of the comment.</param>
    /// <returns>A redirect back to the task details page.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int taskId, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            var added = await this.commentService.AddCommentAsync(taskId, text);
            if (added is null)
            {
                return this.NotFound();
            }
        }

        return this.RedirectToAction("Details", "TodoTask", new { id = taskId });
    }

    /// <summary>
    /// Shows the form used to edit an existing comment.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <returns>The edit comment view.</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var comment = await this.commentService.GetCommentAsync(id);
        if (comment is null)
        {
            return this.NotFound();
        }

        return this.View(comment);
    }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="model">The updated comment data.</param>
    /// <returns>A redirect back to the task details page.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CommentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var updated = await this.commentService.UpdateCommentAsync(model.Id, model.Text);
        if (!updated)
        {
            return this.NotFound();
        }

        return this.RedirectToAction("Details", "TodoTask", new { id = model.TaskId });
    }

    /// <summary>
    /// Shows the confirmation page used to delete an existing comment.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <returns>The delete confirmation view.</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await this.commentService.GetCommentAsync(id);
        if (comment is null)
        {
            return this.NotFound();
        }

        return this.View(comment);
    }

    /// <summary>
    /// Deletes an existing comment.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <param name="taskId">The identifier of the task the comment belongs to.</param>
    /// <returns>A redirect back to the task details page.</returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int taskId)
    {
        var deleted = await this.commentService.DeleteCommentAsync(id);
        if (!deleted)
        {
            return this.NotFound();
        }

        return this.RedirectToAction("Details", "TodoTask", new { id = taskId });
    }
}
