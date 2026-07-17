using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApp.Data;
using TodoListApp.WebApp.Logging;
using TodoListApp.WebApp.Models;
using TodoListApp.WebApp.Services;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles browser requests to manage to-do tasks within a to-do list.
/// </summary>
[Authorize]
public class TodoTaskController : Controller
{
    private const int PageSize = 20;

    private readonly ITodoTaskWebApiService taskService;
    private readonly ITodoListWebApiService todoListService;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<TodoTaskController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskController"/> class.
    /// </summary>
    /// <param name="taskService">The service used to manage tasks.</param>
    /// <param name="todoListService">The service used to manage to-do lists.</param>
    /// <param name="userManager">The user manager, used to resolve assignee display names.</param>
    /// <param name="logger">The logger.</param>
    public TodoTaskController(
        ITodoTaskWebApiService taskService,
        ITodoListWebApiService todoListService,
        UserManager<ApplicationUser> userManager,
        ILogger<TodoTaskController> logger)
    {
        this.taskService = taskService;
        this.todoListService = todoListService;
        this.userManager = userManager;
        this.logger = logger;
    }

    /// <summary>
    /// Shows the list of tasks in the specified to-do list.
    /// </summary>
    /// <param name="todoListId">The identifier of the to-do list.</param>
    /// <param name="pageNumber">The page number, starting from 1.</param>
    /// <returns>The task list view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(int todoListId, int pageNumber = 1)
    {
        var page = await this.taskService.GetTasksAsync(todoListId, pageNumber, PageSize);
        if (page is null)
        {
            return this.NotFound();
        }

        var todoList = await this.todoListService.GetTodoListAsync(todoListId);
        this.ViewBag.TodoListId = todoListId;
        this.ViewBag.TodoListTitle = todoList?.Title ?? string.Empty;

        return this.View(page);
    }

    /// <summary>
    /// Shows the task details page.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <returns>The task details view.</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var task = await this.taskService.GetTaskAsync(id);
        if (task is null)
        {
            return this.NotFound();
        }

        return this.View(await this.ToViewModelAsync(task));
    }

    /// <summary>
    /// Shows the form used to add a new task to a to-do list.
    /// </summary>
    /// <param name="todoListId">The identifier of the to-do list.</param>
    /// <returns>The add task view.</returns>
    [HttpGet]
    public IActionResult Add(int todoListId) => this.View(new TodoTaskModel { TodoListId = todoListId });

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="model">The task data.</param>
    /// <returns>A redirect to the task list on success; otherwise, the form with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Add(TodoTaskModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return this.AddInternalAsync(model);
    }

    /// <summary>
    /// Shows the form used to edit an existing task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <returns>The edit task view.</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var task = await this.taskService.GetTaskAsync(id);
        if (task is null)
        {
            return this.NotFound();
        }

        this.ViewBag.Assignees = await this.GetAssigneeSelectListAsync(task.AssigneeId);

        return this.View(await this.ToViewModelAsync(task));
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="model">The updated task data.</param>
    /// <returns>A redirect to the task details on success; otherwise, the form with validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Edit(int id, TodoTaskModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return this.EditInternalAsync(id, model);
    }

    /// <summary>
    /// Shows the confirmation page used to delete an existing task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <returns>The delete confirmation view.</returns>
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await this.taskService.GetTaskAsync(id);
        if (task is null)
        {
            return this.NotFound();
        }

        return this.View(task);
    }

    /// <summary>
    /// Deletes an existing task.
    /// </summary>
    /// <param name="id">The identifier of the task.</param>
    /// <param name="todoListId">The identifier of the to-do list the task belongs to.</param>
    /// <returns>A redirect to the task list.</returns>
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int todoListId)
    {
        await this.taskService.DeleteTaskAsync(id);
        this.logger.TaskDeleted(id);

        return this.RedirectToAction(nameof(this.Index), new { todoListId });
    }

    private async Task<IActionResult> AddInternalAsync(TodoTaskModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var created = await this.taskService.AddTaskAsync(
            model.TodoListId,
            new TodoTask
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
            });

        if (created is null)
        {
            return this.NotFound();
        }

        return this.RedirectToAction(nameof(this.Index), new { todoListId = model.TodoListId });
    }

    private async Task<IActionResult> EditInternalAsync(int id, TodoTaskModel model)
    {
        if (!this.ModelState.IsValid)
        {
            this.ViewBag.Assignees = await this.GetAssigneeSelectListAsync(model.AssigneeId);
            return this.View(model);
        }

        var updated = await this.taskService.UpdateTaskAsync(
            new TodoTask
            {
                Id = id,
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
                AssigneeId = model.AssigneeId,
            });

        if (!updated)
        {
            return this.NotFound();
        }

        return this.RedirectToAction(nameof(this.Details), new { id });
    }

    private async Task<TodoTaskModel> ToViewModelAsync(TodoTask task)
    {
        var assignee = await this.userManager.FindByIdAsync(task.AssigneeId);

        return new TodoTaskModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedDate = task.CreatedDate,
            DueDate = task.DueDate,
            Status = task.Status,
            TodoListId = task.TodoListId,
            TodoListTitle = task.TodoListTitle,
            AssigneeId = task.AssigneeId,
            AssigneeName = assignee?.DisplayName ?? task.AssigneeId,
            TagCount = task.TagCount,
            CommentCount = task.CommentCount,
            IsOverdue = task.IsOverdue,
        };
    }

    private async Task<SelectList> GetAssigneeSelectListAsync(string selectedId)
    {
        var users = await this.userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.DisplayName)
            .Select(u => new { u.Id, u.DisplayName })
            .ToListAsync();

        return new SelectList(users, nameof(ApplicationUser.Id), nameof(ApplicationUser.DisplayName), selectedId);
    }
}
