using System.Net;
using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Manages to-do tasks in the web API app using the REST API.
/// </summary>
public class TodoTaskWebApiService : ITodoTaskWebApiService
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoTaskWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured to call the web API app.</param>
    public TodoTaskWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>?> GetTasksAsync(int todoListId, string userId, int pageNumber, int pageSize)
    {
        var url = $"api/todolists/{todoListId}/tasks?userId={Uri.EscapeDataString(userId)}&pageNumber={pageNumber}&pageSize={pageSize}";
        using var response = await this.httpClient.GetAsync(new Uri(url, UriKind.Relative));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<PagedResult<TodoTaskWebApiModel>>();

        return new PagedResult<TodoTask>
        {
            Items = page?.Items.Select(ToDomainModel).ToList() ?? new List<TodoTask>(),
            PageNumber = page?.PageNumber ?? pageNumber,
            PageSize = page?.PageSize ?? pageSize,
            TotalCount = page?.TotalCount ?? 0,
        };
    }

    /// <inheritdoc/>
    public async Task<TodoTask?> GetTaskAsync(int id, string userId)
    {
        var url = $"api/tasks/{id}?userId={Uri.EscapeDataString(userId)}";
        using var response = await this.httpClient.GetAsync(new Uri(url, UriKind.Relative));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<TodoTaskWebApiModel>();

        return model is null ? null : ToDomainModel(model);
    }

    /// <inheritdoc/>
    public async Task<TodoTask?> AddTaskAsync(int todoListId, string userId, TodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var url = $"api/todolists/{todoListId}/tasks?userId={Uri.EscapeDataString(userId)}";
        using var response = await this.httpClient.PostAsJsonAsync(url, ToApiModel(task));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TodoTaskWebApiModel>();

        return created is null ? null : ToDomainModel(created);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateTaskAsync(string userId, TodoTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var url = $"api/tasks/{task.Id}?userId={Uri.EscapeDataString(userId)}";
        using var response = await this.httpClient.PutAsJsonAsync(url, ToApiModel(task));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTaskAsync(int id, string userId)
    {
        var url = $"api/tasks/{id}?userId={Uri.EscapeDataString(userId)}";
        using var response = await this.httpClient.DeleteAsync(new Uri(url, UriKind.Relative));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>> GetAssignedTasksAsync(
        string userId,
        TodoTaskStatus? statusFilter,
        bool showAll,
        string? sortBy,
        int pageNumber,
        int pageSize)
    {
        var url = $"api/tasks/assigned?userId={Uri.EscapeDataString(userId)}&showAll={showAll}&pageNumber={pageNumber}&pageSize={pageSize}";
        if (statusFilter.HasValue)
        {
            url += $"&status={(int)statusFilter.Value}";
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            url += $"&sortBy={Uri.EscapeDataString(sortBy)}";
        }

        var page = await this.httpClient.GetFromJsonAsync<PagedResult<TodoTaskWebApiModel>>(url);

        return new PagedResult<TodoTask>
        {
            Items = page?.Items.Select(ToDomainModel).ToList() ?? new List<TodoTask>(),
            PageNumber = page?.PageNumber ?? pageNumber,
            PageSize = page?.PageSize ?? pageSize,
            TotalCount = page?.TotalCount ?? 0,
        };
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateTaskStatusAsync(int id, string userId, TodoTaskStatus status)
    {
        var url = $"api/tasks/{id}/status?userId={Uri.EscapeDataString(userId)}&status={(int)status}";
        using var response = await this.httpClient.PatchAsync(new Uri(url, UriKind.Relative), content: null);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>> SearchTasksAsync(
        string userId,
        string? title,
        DateTime? createdDate,
        DateTime? dueDate,
        int pageNumber,
        int pageSize)
    {
        var url = $"api/tasks/search?userId={Uri.EscapeDataString(userId)}&pageNumber={pageNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(title))
        {
            url += $"&title={Uri.EscapeDataString(title)}";
        }

        if (createdDate.HasValue)
        {
            url += $"&createdDate={createdDate.Value:yyyy-MM-dd}";
        }

        if (dueDate.HasValue)
        {
            url += $"&dueDate={dueDate.Value:yyyy-MM-dd}";
        }

        var page = await this.httpClient.GetFromJsonAsync<PagedResult<TodoTaskWebApiModel>>(url);

        return new PagedResult<TodoTask>
        {
            Items = page?.Items.Select(ToDomainModel).ToList() ?? new List<TodoTask>(),
            PageNumber = page?.PageNumber ?? pageNumber,
            PageSize = page?.PageSize ?? pageSize,
            TotalCount = page?.TotalCount ?? 0,
        };
    }

    private static TodoTask ToDomainModel(TodoTaskWebApiModel model) => new TodoTask
    {
        Id = model.Id,
        Title = model.Title,
        Description = model.Description,
        CreatedDate = model.CreatedDate,
        DueDate = model.DueDate,
        Status = model.Status,
        AssigneeId = model.AssigneeId,
        TodoListId = model.TodoListId,
        TodoListTitle = model.TodoListTitle,
        TagCount = model.TagCount,
        CommentCount = model.CommentCount,
        IsOverdue = model.IsOverdue,
    };

    private static TodoTaskWebApiModel ToApiModel(TodoTask task) => new TodoTaskWebApiModel
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
        IsOverdue = task.IsOverdue,
    };
}
