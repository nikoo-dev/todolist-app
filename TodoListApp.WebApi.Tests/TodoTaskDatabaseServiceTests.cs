using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Services;
using TodoListApp.WebApi.Tests.TestHelpers;

namespace TodoListApp.WebApi.Tests;

[TestFixture]
public class TodoTaskDatabaseServiceTests
{
    private const string OwnerId = "owner-1";
    private const string AssigneeId = "assignee-1";
    private const string StrangerId = "stranger-1";

    [Test]
    public async Task GetTasksAsync_ReturnsNullWhenTheListIsNotOwnedByTheCaller()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var result = await service.GetTasksAsync(list.Id, StrangerId, pageNumber: 1, pageSize: 10);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetTaskAsync_IsAccessibleByBothOwnerAndAssignee()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var task = new TodoTaskEntity { Title = "Task", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today };
        context.TodoTasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);

        Assert.That(await service.GetTaskAsync(task.Id, OwnerId), Is.Not.Null);
        Assert.That(await service.GetTaskAsync(task.Id, AssigneeId), Is.Not.Null);
        Assert.That(await service.GetTaskAsync(task.Id, StrangerId), Is.Null);
    }

    [Test]
    public async Task AddTaskAsync_SelfAssignsTheCreatingOwner()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var created = await service.AddTaskAsync(
            new TodoListApp.WebApi.Models.TodoTask { Title = "New task", TodoListId = list.Id, DueDate = DateTime.Today },
            OwnerId);

        Assert.That(created, Is.Not.Null);
        Assert.That(created!.AssigneeId, Is.EqualTo(OwnerId));
    }

    [Test]
    public async Task UpdateTaskAsync_ReassignsTaskToADifferentAssignee()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var task = new TodoTaskEntity { Title = "Task", TodoListId = list.Id, AssigneeId = OwnerId, DueDate = DateTime.Today };
        context.TodoTasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var updated = await service.UpdateTaskAsync(
            new TodoListApp.WebApi.Models.TodoTask { Id = task.Id, Title = "Task", DueDate = DateTime.Today, AssigneeId = AssigneeId },
            OwnerId);

        Assert.That(updated, Is.True);
        var stored = await context.TodoTasks.FindAsync(task.Id);
        Assert.That(stored?.AssigneeId, Is.EqualTo(AssigneeId));
    }

    [Test]
    public async Task UpdateTaskAsync_ReturnsFalseWhenTheCallerDoesNotOwnTheList()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var task = new TodoTaskEntity { Title = "Task", TodoListId = list.Id, AssigneeId = OwnerId, DueDate = DateTime.Today };
        context.TodoTasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var updated = await service.UpdateTaskAsync(
            new TodoListApp.WebApi.Models.TodoTask { Id = task.Id, Title = "Hijacked", DueDate = DateTime.Today },
            StrangerId);

        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task GetAssignedTasksAsync_DefaultsToActiveTasksOnly()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        context.TodoTasks.Add(new TodoTaskEntity { Title = "Active", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today, Status = TodoTaskStatus.NotStarted });
        context.TodoTasks.Add(new TodoTaskEntity { Title = "Done", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today, Status = TodoTaskStatus.Completed });
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var page = await service.GetAssignedTasksAsync(AssigneeId, statusFilter: null, showAll: false, sortBy: null, pageNumber: 1, pageSize: 10);

        Assert.That(page.Items.Single().Title, Is.EqualTo("Active"));
    }

    [Test]
    public async Task GetAssignedTasksAsync_ShowAllIncludesCompletedTasks()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        context.TodoTasks.Add(new TodoTaskEntity { Title = "Active", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today, Status = TodoTaskStatus.NotStarted });
        context.TodoTasks.Add(new TodoTaskEntity { Title = "Done", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today, Status = TodoTaskStatus.Completed });
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var page = await service.GetAssignedTasksAsync(AssigneeId, statusFilter: null, showAll: true, sortBy: null, pageNumber: 1, pageSize: 10);

        Assert.That(page.TotalCount, Is.EqualTo(2));
    }

    [Test]
    public async Task GetAssignedTasksAsync_FiltersBySpecificStatus()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        context.TodoTasks.Add(new TodoTaskEntity { Title = "In progress", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today, Status = TodoTaskStatus.InProgress });
        context.TodoTasks.Add(new TodoTaskEntity { Title = "Not started", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today, Status = TodoTaskStatus.NotStarted });
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var page = await service.GetAssignedTasksAsync(AssigneeId, statusFilter: TodoTaskStatus.InProgress, showAll: false, sortBy: null, pageNumber: 1, pageSize: 10);

        Assert.That(page.Items.Single().Title, Is.EqualTo("In progress"));
    }

    [Test]
    public async Task UpdateTaskStatusAsync_OnlyAllowsTheAssignee()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var task = new TodoTaskEntity { Title = "Task", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today, Status = TodoTaskStatus.NotStarted };
        context.TodoTasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);

        Assert.That(await service.UpdateTaskStatusAsync(task.Id, OwnerId, TodoTaskStatus.Completed), Is.False);
        Assert.That(await service.UpdateTaskStatusAsync(task.Id, AssigneeId, TodoTaskStatus.Completed), Is.True);

        var stored = await context.TodoTasks.FindAsync(task.Id);
        Assert.That(stored?.Status, Is.EqualTo(TodoTaskStatus.Completed));
    }

    [Test]
    public async Task SearchTasksAsync_FiltersByTitleAndDueDate()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        context.TodoTasks.Add(new TodoTaskEntity { Title = "Buy milk", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = new DateTime(2026, 1, 1) });
        context.TodoTasks.Add(new TodoTaskEntity { Title = "Buy bread", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = new DateTime(2026, 2, 1) });
        context.TodoTasks.Add(new TodoTaskEntity { Title = "Clean house", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = new DateTime(2026, 1, 1) });
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var page = await service.SearchTasksAsync(AssigneeId, title: "Buy", createdDate: null, dueDate: new DateTime(2026, 1, 1), pageNumber: 1, pageSize: 10);

        Assert.That(page.Items.Single().Title, Is.EqualTo("Buy milk"));
    }

    [Test]
    public async Task DeleteTaskAsync_ReturnsFalseWhenTheCallerDoesNotOwnTheList()
    {
        using var context = TestDbContextFactory.Create();
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var task = new TodoTaskEntity { Title = "Task", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today };
        context.TodoTasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TodoTaskDatabaseService(context);
        var deleted = await service.DeleteTaskAsync(task.Id, AssigneeId);

        Assert.That(deleted, Is.False);
        Assert.That(await context.TodoTasks.FindAsync(task.Id), Is.Not.Null);
    }
}
