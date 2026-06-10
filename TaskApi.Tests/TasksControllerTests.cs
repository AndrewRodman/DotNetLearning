using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskApi.Controllers;
using TaskApi.Models;
using TaskApi.Repositories;

namespace TaskApi.Tests;

public class TasksControllerTests
{
    private static TasksController CreateController(ITaskRepository repository, int userId = 1)
    {
        var controller = new TasksController(repository);
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ], authenticationType: "Test");
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
        return controller;
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenTaskMissing()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.GetByIdAsync(1, 42)).ReturnsAsync((TaskItem?)null);

        var controller = CreateController(repository.Object);

        var result = await controller.GetById(42);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_AndSetsUserId()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem task) =>
            {
                task.Id = 7;
                return task;
            });

        var controller = CreateController(repository.Object, userId: 5);

        var result = await controller.Create(new CreateTaskRequest
        {
            Title = "New task",
            Description = "From test"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var task = Assert.IsType<TaskItem>(created.Value);
        Assert.Equal(7, task.Id);
        Assert.Equal(5, task.UserId);
        Assert.Equal("New task", task.Title);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenTaskDeleted()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.DeleteAsync(1, 1)).ReturnsAsync(true);

        var controller = CreateController(repository.Object);

        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenTaskMissing()
    {
        var repository = new Mock<ITaskRepository>();
        repository.Setup(r => r.DeleteAsync(1, 999)).ReturnsAsync(false);

        var controller = CreateController(repository.Object);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}