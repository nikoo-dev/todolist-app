using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST endpoints to manage to-do tasks.
/// </summary>
[ApiController]
[Authorize]
[Route("api")]
public class TodoTaskController : ControllerBase
{
    private readonly ITodoTaskDatabaseService taskService;
    private readonly ILogger<TodoTaskController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskController"/> class.
    /// </summary>
    /// <param name="taskService">The service used to manage tasks.</param>
    /// <param name="logger">The logger.</param>
    public TodoTaskController(ITodoTaskDatabaseService taskService, ILogger<TodoTaskController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    /// <summary>
    /// Gets a page of tasks that belong to the specified to-do list.
    /// </summary>
    /// <param name="todoListId">The identifier of the to-do list.</param>
    /// <param name="userId">The identifier of the to-do list owner.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of tasks.</returns>
    [HttpGet("todolists/{todoListId:int}/tasks")]
    public async Task<ActionResult<PagedResult<TodoTaskModel>>> GetTasks(
        int todoListId,
        [FromQuery] string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var page = await this.taskService.GetTasksAsync(todoListId, userId, pageNumber, pageSize);
        if (page is null)
        {
            return this.NotFound();
        }

        var result = new PagedResult<TodoTaskModel>
        {
            Items = page.Items.Select(ToApiModel).ToList(),
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
        };

        return this.Ok(result);
    }

    /// <summary>
    /// Gets a single task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="userId">The identifier of the to-do list owner.</param>
    /// <returns>The task.</returns>
    [HttpGet("tasks/{id:int}")]
    public async Task<ActionResult<TodoTaskModel>> GetTask(int id, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        var task = await this.taskService.GetTaskAsync(id, userId);
        if (task is null)
        {
            return this.NotFound();
        }

        return this.Ok(ToApiModel(task));
    }

    /// <summary>
    /// Adds a new task to the specified to-do list.
    /// </summary>
    /// <param name="todoListId">The identifier of the to-do list.</param>
    /// <param name="userId">The identifier of the to-do list owner.</param>
    /// <param name="model">The task data.</param>
    /// <returns>The added task.</returns>
    [HttpPost("todolists/{todoListId:int}/tasks")]
    public async Task<ActionResult<TodoTaskModel>> AddTask(int todoListId, [FromQuery] string userId, [FromBody] TodoTaskModel model)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var task = await this.taskService.AddTaskAsync(
            new Models.TodoTask
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
                TodoListId = todoListId,
            },
            userId);

        if (task is null)
        {
            return this.NotFound();
        }

        this.logger.TaskCreated(task.Id, todoListId, userId);

        return this.CreatedAtAction(nameof(this.GetTask), new { id = task.Id, userId }, ToApiModel(task));
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="userId">The identifier of the to-do list owner.</param>
    /// <param name="model">The updated task data.</param>
    /// <returns>No content if the update succeeded.</returns>
    [HttpPut("tasks/{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, [FromQuery] string userId, [FromBody] TodoTaskModel model)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var updated = await this.taskService.UpdateTaskAsync(
            new Models.TodoTask
            {
                Id = id,
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
            },
            userId);

        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Deletes an existing task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="userId">The identifier of the to-do list owner.</param>
    /// <returns>No content if the delete succeeded.</returns>
    [HttpDelete("tasks/{id:int}")]
    public async Task<IActionResult> DeleteTask(int id, [FromQuery] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        var deleted = await this.taskService.DeleteTaskAsync(id, userId);
        if (!deleted)
        {
            return this.NotFound();
        }

        this.logger.TaskDeleted(id, userId);

        return this.NoContent();
    }

    /// <summary>
    /// Gets a page of tasks assigned to the current user.
    /// </summary>
    /// <param name="userId">The identifier of the assignee.</param>
    /// <param name="status">The status to filter by. When omitted, only active tasks are returned unless <paramref name="showAll"/> is <see langword="true"/>.</param>
    /// <param name="showAll">When <see langword="true"/>, tasks of every status are returned.</param>
    /// <param name="sortBy">The field to sort by: <c>"title"</c> or <c>"dueDate"</c> (the default).</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of assigned tasks.</returns>
    [HttpGet("tasks/assigned")]
    public async Task<ActionResult<PagedResult<TodoTaskModel>>> GetAssignedTasks(
        [FromQuery] string userId,
        [FromQuery] TodoTaskStatus? status = null,
        [FromQuery] bool showAll = false,
        [FromQuery] string? sortBy = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var page = await this.taskService.GetAssignedTasksAsync(userId, status, showAll, sortBy, pageNumber, pageSize);

        var result = new PagedResult<TodoTaskModel>
        {
            Items = page.Items.Select(ToApiModel).ToList(),
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
        };

        return this.Ok(result);
    }

    /// <summary>
    /// Updates the status of a task assigned to the current user.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="userId">The identifier of the assignee.</param>
    /// <param name="status">The new status.</param>
    /// <returns>No content if the update succeeded.</returns>
    [HttpPatch("tasks/{id:int}/status")]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromQuery] string userId, [FromQuery] TodoTaskStatus status)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        var updated = await this.taskService.UpdateTaskStatusAsync(id, userId, status);
        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    /// <summary>
    /// Searches the tasks assigned to the current user by title, creation date, or due date.
    /// </summary>
    /// <param name="userId">The identifier of the assignee.</param>
    /// <param name="title">The text to search for in the task title.</param>
    /// <param name="createdDate">The exact creation date to filter by.</param>
    /// <param name="dueDate">The exact due date to filter by.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of matching tasks.</returns>
    [HttpGet("tasks/search")]
    public async Task<ActionResult<PagedResult<TodoTaskModel>>> SearchTasks(
        [FromQuery] string userId,
        [FromQuery] string? title = null,
        [FromQuery] DateTime? createdDate = null,
        [FromQuery] DateTime? dueDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return this.BadRequest("The userId query parameter is required.");
        }

        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var page = await this.taskService.SearchTasksAsync(userId, title, createdDate, dueDate, pageNumber, pageSize);

        var result = new PagedResult<TodoTaskModel>
        {
            Items = page.Items.Select(ToApiModel).ToList(),
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
        };

        return this.Ok(result);
    }

    private static TodoTaskModel ToApiModel(Models.TodoTask task) => new TodoTaskModel
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        CreatedDate = task.CreatedDate,
        DueDate = task.DueDate,
        Status = task.Status,
        AssigneeId = task.AssigneeId,
        TodoListId = task.TodoListId,
        TodoListTitle = task.TodoListTitle,
        TagCount = task.TagCount,
        CommentCount = task.CommentCount,
    };
}
