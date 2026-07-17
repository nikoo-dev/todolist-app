using System.Net;
using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Manages to-do lists in the web API app using the REST API.
/// </summary>
public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured to call the web API app.</param>
    public TodoListWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoList>> GetTodoListsAsync(int pageNumber, int pageSize)
    {
        var url = $"api/todolists?pageNumber={pageNumber}&pageSize={pageSize}";
        var page = await this.httpClient.GetFromJsonAsync<PagedResult<TodoListWebApiModel>>(url);

        return new PagedResult<TodoList>
        {
            Items = page?.Items.Select(ToDomainModel).ToList() ?? new List<TodoList>(),
            PageNumber = page?.PageNumber ?? pageNumber,
            PageSize = page?.PageSize ?? pageSize,
            TotalCount = page?.TotalCount ?? 0,
        };
    }

    /// <inheritdoc/>
    public async Task<TodoList?> GetTodoListAsync(int id)
    {
        using var response = await this.httpClient.GetAsync(new Uri($"api/todolists/{id}", UriKind.Relative));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<TodoListWebApiModel>();

        return model is null ? null : ToDomainModel(model);
    }

    /// <inheritdoc/>
    public async Task<TodoList> AddTodoListAsync(TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        var response = await this.httpClient.PostAsJsonAsync("api/todolists", ToApiModel(todoList));
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<TodoListWebApiModel>();

        return ToDomainModel(created ?? throw new InvalidOperationException("The web API returned an empty response."));
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateTodoListAsync(TodoList todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        var response = await this.httpClient.PutAsJsonAsync($"api/todolists/{todoList.Id}", ToApiModel(todoList));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTodoListAsync(int id)
    {
        using var response = await this.httpClient.DeleteAsync(new Uri($"api/todolists/{id}", UriKind.Relative));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    private static TodoList ToDomainModel(TodoListWebApiModel model) => new TodoList
    {
        Id = model.Id,
        Title = model.Title,
        Description = model.Description,
        TaskCount = model.TaskCount,
    };

    private static TodoListWebApiModel ToApiModel(TodoList todoList) => new TodoListWebApiModel
    {
        Id = todoList.Id,
        Title = todoList.Title,
        Description = todoList.Description,
        TaskCount = todoList.TaskCount,
    };
}
