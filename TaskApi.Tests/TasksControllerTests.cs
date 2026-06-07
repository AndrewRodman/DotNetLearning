using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApi.Controllers;
using TaskApi.Models;
using TaskApi.Repositories;

namespace TaskApi.Tests;

public class TasksControllerTests
{
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenTaskMissing()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((TaskItem?)null);

        var controller = new TasksController(repository.Object);

        var result = await controller.GetById(42);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) =>
            {
                task.Id = 7;
                return task;
            });

        var controller = new TasksController(repository.Object);

        var result = await controller.Create(new CreateTaskRequest
        {
            Title = "New task",
            Description = "From test"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var task = Assert.IsType<TaskItem>(created.Value);
        Assert.Equal(7, task.Id);
        Assert.Equal("New task", task.Title);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenTaskDeleted()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var controller = new TasksController(repository.Object);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenTaskMissing()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        var controller = new TasksController(repository.Object);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}