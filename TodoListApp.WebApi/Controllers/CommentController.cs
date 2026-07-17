using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST endpoints to manage comments.
/// </summary>
[ApiController]
[Authorize]
[Route("api")]
public class CommentController : ControllerBase
{
    private readonly ICommentDatabaseService commentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentController"/> class.
    /// </summary>
    /// <param name="commentService">The service used to manage comments.</param>
    public CommentController(ICommentDatabaseService commentService)
    {
        this.commentService = commentService;
    }

    private string CurrentUserId =>
        this.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("The authenticated request is missing its user identifier claim.");

    /// <summary>
    /// Gets the comments added to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <returns>The comments.</returns>
    [HttpGet("tasks/{taskId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentModel>>> GetComments(int taskId)
    {
        var comments = await this.commentService.GetCommentsForTaskAsync(taskId, this.CurrentUserId);
        if (comments is null)
        {
            return this.NotFound();
        }

        return this.Ok(comments);
    }

    /// <summary>
    /// Gets a single comment the current user is allowed to edit or delete.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <returns>The comment.</returns>
    [HttpGet("comments/{id:int}")]
    public async Task<ActionResult<CommentModel>> GetComment(int id)
    {
        var comment = await this.commentService.GetCommentAsync(id, this.CurrentUserId);
        if (comment is null)
        {
            return this.NotFound();
        }

        return this.Ok(comment);
    }

    /// <summary>
    /// Adds a new comment to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="model">The comment data.</param>
    /// <returns>The added comment.</returns>
    [HttpPost("tasks/{taskId:int}/comments")]
    public async Task<ActionResult<CommentModel>> AddComment(int taskId, [FromBody] CommentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var comment = await this.commentService.AddCommentAsync(taskId, this.CurrentUserId, model.Text);
        if (comment is null)
        {
            return this.NotFound();
        }

        return this.CreatedAtAction(nameof(this.GetComments), new { taskId }, comment);
    }

    /// <summary>
    /// Updates an existing comment. Allowed for the to-do list owner or the comment's own author.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <param name="model">The updated comment data.</param>
    /// <returns>No content if the update succeeded.</returns>
    [HttpPut("comments/{id:int}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var updated = await this.commentService.UpdateCommentAsync(id, this.CurrentUserId, model.Text);
        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Deletes an existing comment. Allowed for the to-do list owner or the comment's own author.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <returns>No content if the delete succeeded.</returns>
    [HttpDelete("comments/{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var deleted = await this.commentService.DeleteCommentAsync(id, this.CurrentUserId);
        if (!deleted)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}
