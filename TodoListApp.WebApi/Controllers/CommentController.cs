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

    /// <summary>
    /// Gets the comments added to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>The comments.</returns>
    [HttpGet("tasks/{taskId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentModel>>> GetComments(int taskId, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        var comments = await this.commentService.GetCommentsForTaskAsync(taskId, userId);
        if (comments is null)
        {
            return this.NotFound();
        }

        return this.Ok(comments);
    }

    /// <summary>
    /// Adds a new comment to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="userId">The identifier of the comment author.</param>
    /// <param name="model">The comment data.</param>
    /// <returns>The added comment.</returns>
    [HttpPost("tasks/{taskId:int}/comments")]
    public async Task<ActionResult<CommentModel>> AddComment(int taskId, [FromQuery] string userId, [FromBody] CommentModel model)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var comment = await this.commentService.AddCommentAsync(taskId, userId, model.Text);
        if (comment is null)
        {
            return this.NotFound();
        }

        return this.CreatedAtAction(nameof(this.GetComments), new { taskId, userId }, comment);
    }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <param name="userId">The identifier of the to-do list owner.</param>
    /// <param name="model">The updated comment data.</param>
    /// <returns>No content if the update succeeded.</returns>
    [HttpPut("comments/{id:int}")]
    public async Task<IActionResult> UpdateComment(int id, [FromQuery] string userId, [FromBody] CommentModel model)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var updated = await this.commentService.UpdateCommentAsync(id, userId, model.Text);
        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Deletes an existing comment.
    /// </summary>
    /// <param name="id">The identifier of the comment.</param>
    /// <param name="userId">The identifier of the to-do list owner.</param>
    /// <returns>No content if the delete succeeded.</returns>
    [HttpDelete("comments/{id:int}")]
    public async Task<IActionResult> DeleteComment(int id, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        var deleted = await this.commentService.DeleteCommentAsync(id, userId);
        if (!deleted)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}
