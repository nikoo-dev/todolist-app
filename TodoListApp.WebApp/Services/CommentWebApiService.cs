using System.Net;
using System.Net.Http.Json;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

/// <summary>
/// Manages comments in the web API app using the REST API.
/// </summary>
public class CommentWebApiService : ICommentWebApiService
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentWebApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client configured to call the web API app.</param>
    public CommentWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CommentModel>> GetCommentsAsync(int taskId)
    {
        var comments = await this.httpClient.GetFromJsonAsync<List<CommentModel>>($"api/tasks/{taskId}/comments");

        return comments ?? new List<CommentModel>();
    }

    /// <inheritdoc/>
    public async Task<CommentModel?> GetCommentAsync(int commentId)
    {
        using var response = await this.httpClient.GetAsync(new Uri($"api/comments/{commentId}", UriKind.Relative));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CommentModel>();
    }

    /// <inheritdoc/>
    public async Task<CommentModel?> AddCommentAsync(int taskId, string text)
    {
        using var response = await this.httpClient.PostAsJsonAsync($"api/tasks/{taskId}/comments", new CommentModel { Text = text });

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CommentModel>();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateCommentAsync(int commentId, string text)
    {
        using var response = await this.httpClient.PutAsJsonAsync($"api/comments/{commentId}", new CommentModel { Text = text });

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        using var response = await this.httpClient.DeleteAsync(new Uri($"api/comments/{commentId}", UriKind.Relative));

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }
}
