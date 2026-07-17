using TodoListApp.WebApi.Data;
using TodoListApp.WebApi.Entities;
using TodoListApp.WebApi.Services;
using TodoListApp.WebApi.Tests.TestHelpers;

namespace TodoListApp.WebApi.Tests;

[TestFixture]
public class CommentDatabaseServiceTests
{
    private const string OwnerId = "owner-1";
    private const string AssigneeId = "assignee-1";
    private const string StrangerId = "stranger-1";

    [Test]
    public async Task AddCommentAsync_AllowsTheAssigneeToComment()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);

        var service = new CommentDatabaseService(context);
        var comment = await service.AddCommentAsync(task.Id, AssigneeId, "First comment");

        Assert.That(comment, Is.Not.Null);
        Assert.That(comment!.AuthorId, Is.EqualTo(AssigneeId));
    }

    [Test]
    public async Task AddCommentAsync_ReturnsNullWhenTheCallerHasNoAccess()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);

        var service = new CommentDatabaseService(context);
        var comment = await service.AddCommentAsync(task.Id, StrangerId, "Should not be added");

        Assert.That(comment, Is.Null);
    }

    [Test]
    public async Task GetCommentsForTaskAsync_ReturnsNullWhenTheCallerHasNoAccess()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);

        var service = new CommentDatabaseService(context);
        var comments = await service.GetCommentsForTaskAsync(task.Id, StrangerId);

        Assert.That(comments, Is.Null);
    }

    [Test]
    public async Task GetCommentAsync_IsEditableByTheListOwnerEvenIfNotTheAuthor()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new CommentDatabaseService(context);
        var added = await service.AddCommentAsync(task.Id, AssigneeId, "Assignee's comment");

        var asOwner = await service.GetCommentAsync(added!.Id, OwnerId);

        Assert.That(asOwner, Is.Not.Null);
    }

    [Test]
    public async Task GetCommentAsync_IsEditableByItsAuthorEvenIfNotTheOwner()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new CommentDatabaseService(context);
        var added = await service.AddCommentAsync(task.Id, AssigneeId, "Assignee's comment");

        var asAuthor = await service.GetCommentAsync(added!.Id, AssigneeId);

        Assert.That(asAuthor, Is.Not.Null);
    }

    [Test]
    public async Task GetCommentAsync_ReturnsNullForAnUnrelatedUser()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new CommentDatabaseService(context);
        var added = await service.AddCommentAsync(task.Id, AssigneeId, "Assignee's comment");

        var asStranger = await service.GetCommentAsync(added!.Id, StrangerId);

        Assert.That(asStranger, Is.Null);
    }

    [Test]
    public async Task UpdateCommentAsync_ReturnsFalseForAnUnrelatedUser()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new CommentDatabaseService(context);
        var added = await service.AddCommentAsync(task.Id, AssigneeId, "Original text");

        var updated = await service.UpdateCommentAsync(added!.Id, StrangerId, "Hijacked text");

        Assert.That(updated, Is.False);
    }

    [Test]
    public async Task UpdateCommentAsync_AllowsTheOwnerToEditSomeoneElsesComment()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new CommentDatabaseService(context);
        var added = await service.AddCommentAsync(task.Id, AssigneeId, "Original text");

        var updated = await service.UpdateCommentAsync(added!.Id, OwnerId, "Edited by owner");

        Assert.That(updated, Is.True);
        var stored = await context.Comments.FindAsync(added.Id);
        Assert.That(stored?.Text, Is.EqualTo("Edited by owner"));
    }

    [Test]
    public async Task DeleteCommentAsync_ReturnsFalseForAnUnrelatedUser()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new CommentDatabaseService(context);
        var added = await service.AddCommentAsync(task.Id, AssigneeId, "Original text");

        var deleted = await service.DeleteCommentAsync(added!.Id, StrangerId);
        var storedComment = await context.Comments.FindAsync(added.Id);

        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.False);
            Assert.That(storedComment, Is.Not.Null);
        });
    }

    [Test]
    public async Task DeleteCommentAsync_AllowsItsAuthorToDeleteIt()
    {
        using var context = TestDbContextFactory.Create();
        var task = await SeedAccessibleTaskAsync(context);
        var service = new CommentDatabaseService(context);
        var added = await service.AddCommentAsync(task.Id, AssigneeId, "Original text");

        var deleted = await service.DeleteCommentAsync(added!.Id, AssigneeId);
        var storedComment = await context.Comments.FindAsync(added.Id);

        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.True);
            Assert.That(storedComment, Is.Null);
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
