using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Logging;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser requests to manage to-do lists.
/// </summary>
[Authorize]
public class TodoListController : Controller
{
    private const int PageSize = 10;

    private readonly ITodoListWebApiService todoListService;
    private readonly ILogger<TodoListController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListController"/> class.
    /// </summary>
    /// <param name="todoListService">The service used to manage to-do lists.</param>
    /// <param name="logger">The logger.</param>
    public TodoListController(ITodoListWebApiService todoListService, ILogger<TodoListController> logger)
    {
        this.todoListService = todoListService;
        this.logger = logger;
    }

    /// <summary>
    /// Shows the list of to-do lists owned by the current user.
    /// </summary>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <returns>The to-do lists view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1)
    {
        var page = await this.todoListService.GetTodoListsAsync(pageNumber, PageSize);

        return this.View(page);
    }

    /// <summary>
    /// Shows the form used to create a new to-do list.
    /// </summary>
    /// <returns>The add to-do list view.</returns>
    [HttpGet]
    public IActionResult Add() => this.View(new TodoListModel());

    /// <summary>
    /// Creates a new to-do list.
    /// </summary>
    /// <param name="model">The to-do list data.</param>
    /// <returns>A redirect to the to-do list index on success; otherwise, the form with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        await this.todoListService.AddTodoListAsync(new TodoList
        {
            Title = model.Title,
            Description = model.Description,
        });

        return this.RedirectToAction(nameof(this.Index));
    }

    /// <summary>
    /// Shows the form used to edit an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <returns>The edit to-do list view.</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var todoList = await this.todoListService.GetTodoListAsync(id);
        if (todoList is null)
        {
            return this.NotFound();
        }

        return this.View(new TodoListModel
        {
            Id = todoList.Id,
            Title = todoList.Title,
            Description = todoList.Description,
            TaskCount = todoList.TaskCount,
        });
    }

    /// <summary>
    /// Updates an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <param name="model">The updated to-do list data.</param>
    /// <returns>A redirect to the to-do list index on success; otherwise, the form with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var updated = await this.todoListService.UpdateTodoListAsync(new TodoList
        {
            Id = id,
            Title = model.Title,
            Description = model.Description,
        });

        if (!updated)
        {
            return this.NotFound();
        }

        return this.RedirectToAction(nameof(this.Index));
    }

    /// <summary>
    /// Shows the confirmation page used to delete an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <returns>The delete confirmation view.</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var todoList = await this.todoListService.GetTodoListAsync(id);
        if (todoList is null)
        {
            return this.NotFound();
        }

        return this.View(todoList);
    }

    /// <summary>
    /// Deletes an existing to-do list.
    /// </summary>
    /// <param name="id">The identifier of the to-do list.</param>
    /// <returns>A redirect to the to-do list index.</returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await this.todoListService.DeleteTodoListAsync(id);
        this.logger.TodoListDeleted(id);

        return this.RedirectToAction(nameof(this.Index));
    }
}
