using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Services;
using TodoListApp.WebApi.Tests.TestHelpers;

namespace TodoListApp.WebApi.Tests;

[TestFixture]
public class TodoListDatabaseServiceTests
{
    private const string OwnerId = "owner-1";
    private const string OtherOwnerId = "owner-2";

    [Test]
    public async Task GetTodoListsAsync_OnlyReturnsListsOwnedByTheSpecifiedUser()
    {
        using var context = TestDbContextFactory.Create();
        context.TodoLists.Add(new TodoListEntity { Title = "Mine", OwnerId = OwnerId });
        context.TodoLists.Add(new TodoListEntity { Title = "Not mine", OwnerId = OtherOwnerId });
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);
        var page = await service.GetTodoListsAsync(OwnerId, pageNumber: 1, pageSize: 10);

        Assert.Multiple(() =>
        {
            Assert.That(page.TotalCount, Is.EqualTo(1));
            Assert.That(page.Items.Single().Title, Is.EqualTo("Mine"));
        });
    }

    [Test]
    public async Task GetTodoListsAsync_PaginatesResults()
    {
        using var context = TestDbContextFactory.Create();
        for (var i = 1; i <= 5; i++)
        {
            context.TodoLists.Add(new TodoListEntity { Title = $"List {i}", OwnerId = OwnerId });
        }

        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);
        var page = await service.GetTodoListsAsync(OwnerId, pageNumber: 2, pageSize: 2);

        Assert.Multiple(() =>
        {
            Assert.That(page.TotalCount, Is.EqualTo(5));
            Assert.That(page.Items, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public async Task GetTodoListAsync_ReturnsNullForAnotherUsersList()
    {
        using var context = TestDbContextFactory.Create();
        var entity = new TodoListEntity { Title = "Private", OwnerId = OtherOwnerId };
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);
        var result = await service.GetTodoListAsync(entity.Id, OwnerId);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetTodoListAsync_ReturnsTaskCount()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "With tasks", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        context.TodoTasks.Add(new TodoTaskEntity { Title = "Task 1", TodoListId = list.Id, AssigneeId = OwnerId, DueDate = DateTime.Today });
        context.TodoTasks.Add(new TodoTaskEntity { Title = "Task 2", TodoListId = list.Id, AssigneeId = OwnerId, DueDate = DateTime.Today });
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);
        var result = await service.GetTodoListAsync(list.Id, OwnerId);

        Assert.That(result?.TaskCount, Is.EqualTo(2));
    }

    [Test]
    public async Task AddTodoListAsync_PersistsWithTheGivenOwner()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TodoListDatabaseService(context);

        var created = await service.AddTodoListAsync(new TodoListApp.WebApi.Models.TodoList
        {
            Title = "New list",
            Description = "Description",
            OwnerId = OwnerId,
        });

        Assert.That(created.Id, Is.GreaterThan(0));
        var stored = await context.TodoLists.FindAsync(created.Id);
        Assert.That(stored?.OwnerId, Is.EqualTo(OwnerId));
    }

    [Test]
    public async Task UpdateTodoListAsync_ReturnsFalseForAnotherUsersList()
    {
        using var context = TestDbContextFactory.Create();
        var entity = new TodoListEntity { Title = "Original", OwnerId = OtherOwnerId };
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);
        var updated = await service.UpdateTodoListAsync(new TodoListApp.WebApi.Models.TodoList
        {
            Id = entity.Id,
            Title = "Hijacked",
            OwnerId = OwnerId,
        });

        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task DeleteTodoListAsync_ReturnsFalseForAnotherUsersList()
    {
        using var context = TestDbContextFactory.Create();
        var entity = new TodoListEntity { Title = "Original", OwnerId = OtherOwnerId };
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);
        var deleted = await service.DeleteTodoListAsync(entity.Id, OwnerId);
        var storedList = await context.TodoLists.FindAsync(entity.Id);

        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.False);
            Assert.That(storedList, Is.Not.Null);
        });
    }

    [Test]
    public async Task DeleteTodoListAsync_RemovesTheListForItsOwner()
    {
        using var context = TestDbContextFactory.Create();
        var entity = new TodoListEntity { Title = "Original", OwnerId = OwnerId };
        context.TodoLists.Add(entity);
        await context.SaveChangesAsync();

        var service = new TodoListDatabaseService(context);
        var deleted = await service.DeleteTodoListAsync(entity.Id, OwnerId);
        var storedList = await context.TodoLists.FindAsync(entity.Id);

        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.True);
            Assert.That(storedList, Is.Null);
        });
    }
}
