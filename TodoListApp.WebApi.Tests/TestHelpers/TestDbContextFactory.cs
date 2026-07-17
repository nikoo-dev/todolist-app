using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;

namespace TodoListApp.WebApi.Tests.TestHelpers;

internal static class TestDbContextFactory
{
    public static TodoListDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TodoListDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TodoListDbContext(options);
    }
}
