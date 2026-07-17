using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;

/// <summary>
/// Handles error page requests.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Shows the error page.
    /// </summary>
    /// <returns>The error view.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
