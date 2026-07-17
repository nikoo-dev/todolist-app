using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Services;

namespace TodoListApp.WebApi.Controllers;

/// <summary>
/// Provides REST endpoints to manage to-do lists.
/// </summary>
[ApiController]
[Authorize]
[Route("api/todolists")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService todoListService;
    private readonly ILogger<TodoListController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="todoListService">The service used to manage to-do lists.</param>
    /// <param name="logger">The logger.</param>
    public TodoListController(ITodoListDatabaseService todoListService, ILogger<TodoListController> logger)
    {
        this.todoListService = todoListService;
        this.logger = logger;
    }

    private string CurrentUserId =>
        this.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("The authenticated request is missing its user identifier claim.");

    /// <summary>
    /// Gets a page of to-do lists owned by the current user.
    /// </summary>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A page of to-do lists.</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<TodoListModel>>> GetTodoLists(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var page = await this.todoListService.GetTodoListsAsync(this.CurrentUserId, pageNumber, pageSize);

        var result = new PagedResult<TodoListModel>
        {
            Items = page.Items.Select(ToApiModel).ToList(),
            PageNumber = page.PageNumber,
            PageSize = page.PageSize,
            TotalCount = page.TotalCount,
        };

        return this.Ok(result);
    }

    /// <summary>
    /// Gets a single to-do list owned by the current user.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <returns>The to-do list.</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoListModel>> GetTodoList(int id)
    {
        var todoList = await this.todoListService.GetTodoListAsync(id, this.CurrentUserId);
        if (todoList is null)
        {
            return this.NotFound();
        }

        return this.Ok(ToApiModel(todoList));
    }

    /// <summary>
    /// Adds a new to-do list.
    /// </summary>
    /// <param name="model">The to-do list data.</param>
    /// <returns>The added to-do list.</returns>
    [HttpPost]
    public Task<ActionResult<TodoListModel>> AddTodoList([FromBody] TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return this.AddTodoListInternalAsync(model);
    }

    /// <summary>
    /// Updates an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <param name="model">The updated to-do list data.</param>
    /// <returns>No content if the update succeeded.</returns>
    [HttpPut("{id:int}")]
    public Task<IActionResult> UpdateTodoList(int id, [FromBody] TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return this.UpdateTodoListInternalAsync(id, model);
    }

    /// <summary>
    /// Deletes an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <returns>No content if the delete succeeded.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodoList(int id)
    {
        var userId = this.CurrentUserId;
        var deleted = await this.todoListService.DeleteTodoListAsync(id, userId);
        if (!deleted)
        {
            return this.NotFound();
        }

        this.logger.TodoListDeleted(id, userId);

        return this.NoContent();
    }

    private static TodoListModel ToApiModel(Models.TodoList todoList) => new TodoListModel
    {
        Id = todoList.Id,
        Title = todoList.Title,
        Description = todoList.Description,
        TaskCount = todoList.TaskCount,
    };

    private async Task<ActionResult<TodoListModel>> AddTodoListInternalAsync(TodoListModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var userId = this.CurrentUserId;
        var todoList = await this.todoListService.AddTodoListAsync(new Models.TodoList
        {
            Title = model.Title,
            Description = model.Description,
            OwnerId = userId,
        });

        this.logger.TodoListCreated(todoList.Id, userId);

        return this.CreatedAtAction(nameof(this.GetTodoList), new { id = todoList.Id }, ToApiModel(todoList));
    }

    private async Task<IActionResult> UpdateTodoListInternalAsync(int id, TodoListModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var updated = await this.todoListService.UpdateTodoListAsync(new Models.TodoList
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
            OwnerId = this.CurrentUserId,
        });

        if (!updated)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}
