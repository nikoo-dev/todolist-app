using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST endpoints to manage tags.
/// </summary>
[ApiController]
[Authorize]
[Route("api")]
public class TagController : ControllerBase
{
    private readonly ITagDatabaseService tagService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagController"/> class.
    /// </summary>
    /// <param name="tagService">The service used to manage tags.</param>
    public TagController(ITagDatabaseService tagService)
    {
        this.tagService = tagService;
    }

    private string CurrentUserId =>
        this.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("The authenticated request is missing its user identifier claim.");

    /// <summary>
    /// Gets all tags added to tasks the current user has access to.
    /// </summary>
    /// <returns>The tags.</returns>
    [HttpGet("tags")]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetAllTags() =>
        this.Ok(await this.tagService.GetAllTagsAsync(this.CurrentUserId));

    /// <summary>
    /// Gets a page of tasks tagged with the specified tag.
    /// </summary>
    /// <param name="tagId">The identifier of the tag.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of tasks.</returns>
    [HttpGet("tags/{tagId:int}/tasks")]
    public async Task<ActionResult<PagedResult<TodoTaskModel>>> GetTasksByTag(
        int tagId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var page = await this.tagService.GetTasksByTagAsync(tagId, this.CurrentUserId, pageNumber, pageSize);
        var result = new PagedResult<TodoTaskModel>
        {
            Items = page.Items.Select(t => new TodoTaskModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate,
                Status = t.Status,
                AssigneeId = t.AssigneeId,
                TodoListId = t.TodoListId,
                TodoListTitle = t.TodoListTitle,
                TagCount = t.TagCount,
                CommentCount = t.CommentCount,
            }).ToList(),
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
        };

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the tags added to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <returns>The tags.</returns>
    [HttpGet("tasks/{taskId:int}/tags")]
    public async Task<ActionResult<IEnumerable<TagModel>>> GetTagsForTask(int taskId)
    {
        var tags = await this.tagService.GetTagsForTaskAsync(taskId, this.CurrentUserId);
        if (tags is null)
        {
            return this.NotFound();
        }

        return this.Ok(tags);
    }

    /// <summary>
    /// Adds a tag to a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="model">The tag data.</param>
    /// <returns>The added tag.</returns>
    [HttpPost("tasks/{taskId:int}/tags")]
    public async Task<ActionResult<TagModel>> AddTagToTask(int taskId, [FromBody] TagModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var tag = await this.tagService.AddTagToTaskAsync(taskId, this.CurrentUserId, model.Name);
        if (tag is null)
        {
            return this.NotFound();
        }

        return this.CreatedAtAction(nameof(this.GetTagsForTask), new { taskId }, tag);
    }

    /// <summary>
    /// Removes a tag from a task.
    /// </summary>
    /// <param name="taskId">The identifier of the task.</param>
    /// <param name="tagId">The identifier of the tag.</param>
    /// <returns>No content if the removal succeeded.</returns>
    [HttpDelete("tasks/{taskId:int}/tags/{tagId:int}")]
    public async Task<IActionResult> RemoveTagFromTask(int taskId, int tagId)
    {
        var removed = await this.tagService.RemoveTagFromTaskAsync(taskId, tagId, this.CurrentUserId);
        if (!removed)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}
