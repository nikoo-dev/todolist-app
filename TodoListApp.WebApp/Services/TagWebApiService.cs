using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Manages tags in the web API app using the REST API.
/// </summary>
public class TagWebApiService : ITagWebApiService
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured to call the web API app.</param>
    public TagWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TagModel>> GetAllTagsAsync(string userId)
    {
        var url = $"api/tags?userId={Uri.EscapeDataString(userId)}";
        var tags = await this.httpClient.GetFromJsonAsync<List<TagModel>>(url);

        return tags ?? new List<TagModel>();
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>> GetTasksByTagAsync(int tagId, string userId, int pageNumber, int pageSize)
    {
        var url = $"api/tags/{tagId}/tasks?userId={Uri.EscapeDataString(userId)}&pageNumber={pageNumber}&pageSize={pageSize}";
        var page = await this.httpClient.GetFromJsonAsync<PagedResult<TodoTaskWebApiModel>>(url);

        return new PagedResult<TodoTask>
        {
            Items = page?.Items.Select(m => new TodoTask
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                CreatedDate = m.CreatedDate,
                DueDate = m.DueDate,
                Status = m.Status,
                AssigneeId = m.AssigneeId,
                TodoListId = m.TodoListId,
                TodoListTitle = m.TodoListTitle,
                TagCount = m.TagCount,
                CommentCount = m.CommentCount,
                IsOverdue = m.IsOverdue,
            }).ToList() ?? new List<TodoTask>(),
            PageNumber = page?.PageNumber ?? pageNumber,
            PageSize = page?.PageSize ?? pageSize,
            TotalCount = page?.TotalCount ?? 0,
        };
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TagModel>> GetTagsForTaskAsync(int taskId, string userId)
    {
        var url = $"api/tasks/{taskId}/tags?userId={Uri.EscapeDataString(userId)}";
        var tags = await this.httpClient.GetFromJsonAsync<List<TagModel>>(url);

        return tags ?? new List<TagModel>();
    }

    /// <inheritdoc/>
    public async Task<TagModel> AddTagToTaskAsync(int taskId, string userId, string tagName)
    {
        var url = $"api/tasks/{taskId}/tags?userId={Uri.EscapeDataString(userId)}";
        var response = await this.httpClient.PostAsJsonAsync(url, new TagModel { Name = tagName });
        response.EnsureSuccessStatusCode();

        var tag = await response.Content.ReadFromJsonAsync<TagModel>();

        return tag ?? throw new InvalidOperationException("The web API returned an empty response.");
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId, string userId)
    {
        var url = $"api/tasks/{taskId}/tags/{tagId}?userId={Uri.EscapeDataString(userId)}";
        using var response = await this.httpClient.DeleteAsync(new Uri(url, UriKind.Relative));
        response.EnsureSuccessStatusCode();

        return true;
    }
}
