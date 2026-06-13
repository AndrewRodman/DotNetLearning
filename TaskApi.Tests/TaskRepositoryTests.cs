using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Repositories;

namespace TaskApi.Tests;

public class TaskRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static async Task<User> SeedUserAsync(AppDbContext db, string username)
    {
        var user = new User { Username = username, PasswordHash = "hash" };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task AddAsync_PersistsTask()
    {
        await using var db = CreateContext();
        var user = await SeedUserAsync(db, "tester");
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem
        {
            UserId = user.Id,
            Title = "Write unit tests",
            Description = "Stage 3"
        });

        Assert.True(created.Id > 0);
        Assert.Equal("Write unit tests", created.Title);
        Assert.Equal(user.Id, created.UserId);
    }

    [Fact]
    public async Task AddAsync_PersistsTaskDueDate()
    {
        await using var db = CreateContext();
        var user = await SeedUserAsync(db, "tester");
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem
        {
            UserId = user.Id,
            Title = "Write unit tests",
            Description = "Stage 3",
            DueDate= DateTime.UtcNow
        });

        Assert.True(created.Id > 0);
        Assert.Equal("Write unit tests", created.Title);
        Assert.Equal(user.Id, created.UserId);
        Assert.Equal(created.DueDate?.Date, DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByIsComplete()
    {
        await using var db = CreateContext();
        var user = await SeedUserAsync(db, "tester");
        var repository = new TaskRepository(db);

        await repository.AddAsync(new TaskItem { UserId = user.Id, Title = "Done", IsComplete = true });
        await repository.AddAsync(new TaskItem { UserId = user.Id, Title = "Todo", IsComplete = false });

        var completed = await repository.GetAllAsync(user.Id, isComplete: true);
        var incomplete = await repository.GetAllAsync(user.Id, isComplete: false);

        Assert.Single(completed);
        Assert.Equal("Done", completed[0].Title);
        Assert.Single(incomplete);
        Assert.Equal("Todo", incomplete[0].Title);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyTasksForUser()
    {
        await using var db = CreateContext();
        var userA = await SeedUserAsync(db, "user_a");
        var userB = await SeedUserAsync(db, "user_b");
        var repository = new TaskRepository(db);

        await repository.AddAsync(new TaskItem { UserId = userA.Id, Title = "A task" });
        await repository.AddAsync(new TaskItem { UserId = userB.Id, Title = "B task" });

        var userATasks = await repository.GetAllAsync(userA.Id);

        Assert.Single(userATasks);
        Assert.Equal("A task", userATasks[0].Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTask()
    {
        await using var db = CreateContext();
        var user = await SeedUserAsync(db, "tester");
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem { UserId = user.Id, Title = "Old title" });

        var updated = await repository.UpdateAsync(user.Id, created.Id, new UpdateTaskRequest
        {
            Title = "New title",
            Description = "Updated",
            IsComplete = true,
            DueDate = DateTime.UtcNow.Date
        });

        Assert.NotNull(updated);
        Assert.Equal("New title", updated.Title);
        Assert.True(updated.IsComplete);
        Assert.Equal(updated.DueDate?.Date, DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenTaskBelongsToAnotherUser()
    {
        await using var db = CreateContext();
        var owner = await SeedUserAsync(db, "owner");
        var other = await SeedUserAsync(db, "other");
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem { UserId = owner.Id, Title = "Private" });

        var result = await repository.UpdateAsync(other.Id, created.Id, new UpdateTaskRequest
        {
            Title = "Hacked",
            IsComplete = false
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        await using var db = CreateContext();
        var user = await SeedUserAsync(db, "tester");
        var repository = new TaskRepository(db);

        var result = await repository.UpdateAsync(user.Id, 999, new UpdateTaskRequest
        {
            Title = "Missing",
            IsComplete = false
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTask()
    {
        await using var db = CreateContext();
        var user = await SeedUserAsync(db, "tester");
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem { UserId = user.Id, Title = "Delete me" });

        var deleted = await repository.DeleteAsync(user.Id, created.Id);
        var found = await repository.GetByIdAsync(user.Id, created.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenTaskBelongsToAnotherUser()
    {
        await using var db = CreateContext();
        var owner = await SeedUserAsync(db, "owner");
        var other = await SeedUserAsync(db, "other");
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem { UserId = owner.Id, Title = "Private" });

        var deleted = await repository.DeleteAsync(other.Id, created.Id);
        var found = await repository.GetByIdAsync(owner.Id, created.Id);

        Assert.False(deleted);
        Assert.NotNull(found);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        await using var db = CreateContext();
        var user = await SeedUserAsync(db, "tester");
        var repository = new TaskRepository(db);

        var deleted = await repository.DeleteAsync(user.Id, 999);

        Assert.False(deleted);
    }
}