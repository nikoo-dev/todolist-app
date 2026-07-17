using Microsoft.EntityFrameworkCore;
using TodoListApp.WebApi.Entities;

namespace TodoListApp.WebApi.Data;

/// <summary>
/// The database context used to access the to-do list database.
/// </summary>
public class TodoListDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the to-do lists stored in the database.
    /// </summary>
    public DbSet<TodoListEntity> TodoLists => this.Set<TodoListEntity>();

    /// <summary>
    /// Gets or sets the to-do tasks stored in the database.
    /// </summary>
    public DbSet<TodoTaskEntity> TodoTasks => this.Set<TodoTaskEntity>();

    /// <summary>
    /// Gets or sets the tags stored in the database.
    /// </summary>
    public DbSet<TagEntity> Tags => this.Set<TagEntity>();

    /// <summary>
    /// Gets or sets the comments stored in the database.
    /// </summary>
    public DbSet<CommentEntity> Comments => this.Set<CommentEntity>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<TodoListEntity>(builder =>
        {
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.OwnerId).IsRequired();
            builder
                .HasMany(e => e.Tasks)
                .WithOne(e => e.TodoList)
                .HasForeignKey(e => e.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TodoTaskEntity>(builder =>
        {
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.AssigneeId).IsRequired();
            builder
                .HasMany(e => e.Comments)
                .WithOne(e => e.Task)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasMany(e => e.Tags)
                .WithMany(e => e.Tasks)
                .UsingEntity(join => join.ToTable("TaskTags"));
        });

        modelBuilder.Entity<TagEntity>(builder =>
        {
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<CommentEntity>(builder =>
        {
            builder.Property(e => e.Text).IsRequired().HasMaxLength(2000);
            builder.Property(e => e.AuthorId).IsRequired();
        });
    }
}
