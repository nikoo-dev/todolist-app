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
    public async Task<IReadOnlyList<CommentModel>> GetCommentsAsync(int taskId, string userId)
    {
        var url = $"api/tasks/{taskId}/comments?userId={Uri.EscapeDataString(userId)}";
        var comments = await this.httpClient.GetFromJsonAsync<List<CommentModel>>(url);

        return comments ?? new List<CommentModel>();
    }

    /// <inheritdoc/>
    public async Task<CommentModel> AddCommentAsync(int taskId, string userId, string text)
    {
        var url = $"api/tasks/{taskId}/comments?userId={Uri.EscapeDataString(userId)}";
        var response = await this.httpClient.PostAsJsonAsync(url, new CommentModel { Text = text });
        response.EnsureSuccessStatusCode();

        var comment = await response.Content.ReadFromJsonAsync<CommentModel>();

        return comment ?? throw new InvalidOperationException("The web API returned an empty response.");
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateCommentAsync(int commentId, string userId, string text)
    {
        var url = $"api/comments/{commentId}?userId={Uri.EscapeDataString(userId)}";
        using var response = await this.httpClient.PutAsJsonAsync(url, new CommentModel { Text = text });

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteCommentAsync(int commentId, string userId)
    {
        var url = $"api/comments/{commentId}?userId={Uri.EscapeDataString(userId)}";
        using var response = await this.httpClient.DeleteAsync(new Uri(url, UriKind.Relative));

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }
}
