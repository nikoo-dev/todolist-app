using Microsoft.AspNetCore.Routing;

namespace TodoListApp.WebApp.ViewModels;

/// <summary>
/// Describes the data needed to render a pagination control.
/// </summary>
public class PaginationViewModel
{
    /// <summary>
    /// Gets or sets the name of the controller the pagination links target.
    /// </summary>
    public string Controller { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the action the pagination links target.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gets or sets additional route values (such as filters) to preserve across page links.
    /// </summary>
    public RouteValueDictionary RouteValues { get; } = new RouteValueDictionary();
}
