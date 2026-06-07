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

    [Fact]
    public async Task AddAsync_PersistsTask()
    {
        await using var db = CreateContext();
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem
        {
            Title = "Write unit tests",
            Description = "Stage 3"
        });

        Assert.True(created.Id > 0);
        Assert.Equal("Write unit tests", created.Title);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByIsComplete()
    {
        await using var db = CreateContext();
        var repository = new TaskRepository(db);

        await repository.AddAsync(new TaskItem { Title = "Done", IsComplete = true });
        await repository.AddAsync(new TaskItem { Title = "Todo", IsComplete = false });

        var completed = await repository.GetAllAsync(isComplete: true);
        var incomplete = await repository.GetAllAsync(isComplete: false);

        Assert.Single(completed);
        Assert.Equal("Done", completed[0].Title);
        Assert.Single(incomplete);
        Assert.Equal("Todo", incomplete[0].Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTask()
    {
        await using var db = CreateContext();
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem { Title = "Old title" });

        var updated = await repository.UpdateAsync(created.Id, new UpdateTaskRequest
        {
            Title = "New title",
            Description = "Updated",
            IsComplete = true
        });

        Assert.NotNull(updated);
        Assert.Equal("New title", updated.Title);
        Assert.True(updated.IsComplete);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        await using var db = CreateContext();
        var repository = new TaskRepository(db);

        var result = await repository.UpdateAsync(999, new UpdateTaskRequest
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
        var repository = new TaskRepository(db);

        var created = await repository.AddAsync(new TaskItem { Title = "Delete me" });

        var deleted = await repository.DeleteAsync(created.Id);
        var found = await repository.GetByIdAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        await using var db = CreateContext();
        var repository = new TaskRepository(db);

        var deleted = await repository.DeleteAsync(999);

        Assert.False(deleted);
    }
}