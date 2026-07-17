using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoListApp.WebApp.Data;

/// <summary>
/// The database context used to store Identity configuration data (users, passwords, and profile data).
/// </summary>
public class UsersDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsersDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
    }
}
