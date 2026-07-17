namespace TodoListApp.WebApp.Models;

/// <summary>
/// Represents a single page of a larger collection of items returned by the web API.
/// </summary>
/// <typeparam name="T">The type of the items in the page.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Gets or sets the items on the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Gets or sets the current page number, starting from 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => this.PageSize == 0 ? 0 : (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);
}
