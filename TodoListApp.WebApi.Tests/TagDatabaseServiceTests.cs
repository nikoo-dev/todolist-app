using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Services;
using TodoListApp.WebApi.Tests.TestHelpers;

namespace TodoListApp.WebApi.Tests;

[TestFixture]
public class TagDatabaseServiceTests
{
    private const string OwnerId = "owner-1";
    private const string AssigneeId = "assignee-1";
    private const string StrangerId = "stranger-1";

    [Test]
    public async Task AddTagToTaskAsync_CreatesANewTagWhenNameIsUnused()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);

        var service = new TagDatabaseService(context);
        var tag = await service.AddTagToTaskAsync(task.Id, OwnerId, "urgent");
        var tagCount = await context.Tags.CountAsync(t => t.Name == "urgent");

        Assert.Multiple(() =>
        {
            Assert.That(tag, Is.Not.Null);
            Assert.That(tagCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task AddTagToTaskAsync_ReusesAnExistingTagByName()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        context.Tags.Add(new TagEntity { Name = "urgent" });
        await context.SaveChangesAsync();

        var service = new TagDatabaseService(context);
        await service.AddTagToTaskAsync(task.Id, OwnerId, "urgent");

        Assert.That(await context.Tags.CountAsync(t => t.Name == "urgent"), Is.EqualTo(1));
    }

    [Test]
    public async Task AddTagToTaskAsync_ReturnsNullWhenTheCallerHasNoAccess()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);

        var service = new TagDatabaseService(context);
        var tag = await service.AddTagToTaskAsync(task.Id, StrangerId, "urgent");

        Assert.That(tag, Is.Null);
    }

    [Test]
    public async Task GetTagsForTaskAsync_ReturnsTagsOrderedByName()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);

        var service = new TagDatabaseService(context);
        await service.AddTagToTaskAsync(task.Id, OwnerId, "zebra");
        await service.AddTagToTaskAsync(task.Id, OwnerId, "alpha");

        var tags = await service.GetTagsForTaskAsync(task.Id, OwnerId);

        Assert.That(tags, Is.Not.Null);
        var expectedNames = new[] { "alpha", "zebra" };
        Assert.That(tags!.Select(t => t.Name), Is.EqualTo(expectedNames));
    }

    [Test]
    public async Task RemoveTagFromTaskAsync_RemovesTheAssociationOnly()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new TagDatabaseService(context);
        var tag = await service.AddTagToTaskAsync(task.Id, OwnerId, "urgent");

        var removed = await service.RemoveTagFromTaskAsync(task.Id, tag!.Id, OwnerId);
        var storedTag = await context.Tags.FindAsync(tag.Id);

        Assert.Multiple(() =>
        {
            Assert.That(removed, Is.True);
            Assert.That(storedTag, Is.Not.Null);
        });

        var tags = await service.GetTagsForTaskAsync(task.Id, OwnerId);
        Assert.That(tags, Is.Empty);
    }

    [Test]
    public async Task RemoveTagFromTaskAsync_ReturnsFalseWhenTheTagIsNotOnTheTask()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new TagDatabaseService(context);

        var removed = await service.RemoveTagFromTaskAsync(task.Id, tagId: 999, OwnerId);

        Assert.That(removed, Is.False);
    }

    [Test]
    public async Task GetAllTagsAsync_OnlyReturnsTagsOnTasksTheUserCanAccess()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new TagDatabaseService(context);
        await service.AddTagToTaskAsync(task.Id, OwnerId, "shared");

        var ownerTags = await service.GetAllTagsAsync(OwnerId);
        var strangerTags = await service.GetAllTagsAsync(StrangerId);

        var expectedNames = new[] { "shared" };
        Assert.Multiple(() =>
        {
            Assert.That(ownerTags.Select(t => t.Name), Is.EqualTo(expectedNames));
            Assert.That(strangerTags, Is.Empty);
        });
    }

    private static async Task<TodoTaskEntity> SeedAccessibleTaskAsync(TodoListDbContext context)
    {
        var list = new TodoListEntity { Title = "List", OwnerId = OwnerId };
        context.TodoLists.Add(list);
        await context.SaveChangesAsync();

        var task = new TodoTaskEntity { Title = "Task", TodoListId = list.Id, AssigneeId = AssigneeId, DueDate = DateTime.Today };
        context.TodoTasks.Add(task);
        await context.SaveChangesAsync();

        return task;
    }
}
