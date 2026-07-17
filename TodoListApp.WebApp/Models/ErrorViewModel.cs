namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents the data shown on the generic error page.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the identifier of the request that failed, used for correlating with server logs.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="RequestId"/> should be displayed.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}
