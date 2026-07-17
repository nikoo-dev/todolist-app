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
    public async Task<IReadOnlyList<TagModel>> GetAllTagsAsync()
    {
        var tags = await this.httpClient.GetFromJsonAsync<List<TagModel>>("api/tags");

        return tags ?? new List<TagModel>();
    }

    /// <inheritdoc/>
    public async Task<PagedResult<TodoTask>> GetTasksByTagAsync(int tagId, int pageNumber, int pageSize)
    {
        var url = $"api/tags/{tagId}/tasks?pageNumber={pageNumber}&pageSize={pageSize}";
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
    public async Task<IReadOnlyList<TagModel>> GetTagsForTaskAsync(int taskId)
    {
        var tags = await this.httpClient.GetFromJsonAsync<List<TagModel>>($"api/tasks/{taskId}/tags");

        return tags ?? new List<TagModel>();
    }

    /// <inheritdoc/>
    public async Task<TagModel> AddTagToTaskAsync(int taskId, string tagName)
    {
        using var response = await this.httpClient.PostAsJsonAsync($"api/tasks/{taskId}/tags", new TagModel { Name = tagName });
        response.EnsureSuccessStatusCode();

        var tag = await response.Content.ReadFromJsonAsync<TagModel>();

        return tag ?? throw new InvalidOperationException("The web API returned an empty response.");
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveTagFromTaskAsync(int taskId, int tagId)
    {
        using var response = await this.httpClient.DeleteAsync(new Uri($"api/tasks/{taskId}/tags/{tagId}", UriKind.Relative));
        response.EnsureSuccessStatusCode();

        return true;
    }
}
