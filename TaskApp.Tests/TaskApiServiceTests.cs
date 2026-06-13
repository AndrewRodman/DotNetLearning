using System.Net;
using System.Net.Http.Headers;
using TaskApp.Models;
using TaskApp.Services;

namespace TaskApp.Tests;

public class TaskApiServiceTests
{
    private static TaskApiService CreateService(
        MockHttpHandler handler,
        FakeSessionContext? session = null,
        string baseAddress = "https://api.test/")
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress)
        };

        return new TaskApiService(httpClient, session ?? new FakeSessionContext());
    }

    [Fact]
    public async Task LoginAsync_ReturnsAuthAndSavesSession_OnSuccess()
    {
        var handler = new MockHttpHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/auth/login", request.RequestUri!.PathAndQuery);

            return MockHttpHandler.JsonResponse(HttpStatusCode.OK, new AuthResponse
            {
                Token = "jwt-123",
                Username = "alice"
            });
        });

        var session = new FakeSessionContext();
        var service = CreateService(handler, session);

        var result = await service.LoginAsync("alice", "Password123");

        Assert.NotNull(result);
        Assert.Equal("jwt-123", result.Token);
        Assert.Equal("alice", result.Username);
        Assert.Equal("jwt-123", session.Token);
        Assert.Equal("alice", session.Username);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_OnFailure()
    {
        var handler = new MockHttpHandler(_ =>
            MockHttpHandler.Empty(HttpStatusCode.Unauthorized));

        var session = new FakeSessionContext();
        var service = CreateService(handler, session);

        var result = await service.LoginAsync("alice", "wrong");

        Assert.Null(result);
        Assert.Null(session.Token);
    }

    [Fact]
    public async Task RegisterAsync_ReturnsAuthAndSavesSession_OnSuccess()
    {
        var handler = new MockHttpHandler(request =>
        {
            Assert.Equal("/api/auth/register", request.RequestUri!.PathAndQuery);

            return MockHttpHandler.JsonResponse(HttpStatusCode.OK, new AuthResponse
            {
                Token = "jwt-456",
                Username = "bob"
            });
        });

        var session = new FakeSessionContext();
        var service = CreateService(handler, session);

        var result = await service.RegisterAsync("bob", "Password123");

        Assert.NotNull(result);
        Assert.Equal("bob", session.Username);
        Assert.Equal("jwt-456", session.Token);
    }

    [Fact]
    public async Task GetTasksAsync_ReturnsAllTasks_WhenNoFilter()
    {
        var handler = new MockHttpHandler(request =>
        {
            Assert.Equal("/api/tasks", request.RequestUri!.PathAndQuery);

            return MockHttpHandler.JsonResponse(HttpStatusCode.OK, new List<TaskItem>
            {
                new() { Id = 1, Title = "One" },
                new() { Id = 2, Title = "Two" }
            });
        });

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var tasks = await service.GetTasksAsync();

        Assert.Equal(2, tasks.Count);
        Assert.Equal("One", tasks[0].Title);
    }

    [Fact]
    public async Task GetTasksAsync_UsesFilterQuery_WhenIsCompleteSet()
    {
        var handler = new MockHttpHandler(request =>
        {
            Assert.Equal("/api/tasks?isComplete=True", request.RequestUri!.PathAndQuery);

            return MockHttpHandler.JsonResponse(HttpStatusCode.OK, new List<TaskItem>
            {
                new() { Id = 3, Title = "Done", IsComplete = true }
            });
        });

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var tasks = await service.GetTasksAsync(isComplete: true);

        Assert.Single(tasks);
        Assert.True(tasks[0].IsComplete);
    }

    [Fact]
    public async Task GetTasksAsync_SetsBearerToken_FromSession()
    {
        var handler = new MockHttpHandler(_ =>
            MockHttpHandler.JsonResponse(HttpStatusCode.OK, new List<TaskItem>()));

        var session = new FakeSessionContext();
        session.Save("saved-token", "alice");
        var service = CreateService(handler, session);

        await service.GetTasksAsync();

        Assert.Equal("Bearer", handler.Requests[0].Headers.Authorization?.Scheme);
        Assert.Equal("saved-token", handler.Requests[0].Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task CreateTaskAsync_ReturnsCreatedTask_OnSuccess()
    {
        var handler = new MockHttpHandler(request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/tasks", request.RequestUri!.PathAndQuery);

            return MockHttpHandler.JsonResponse(HttpStatusCode.Created, new TaskItem
            {
                Id = 9,
                Title = "New task",
                Description = "Details"
            });
        });

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var created = await service.CreateTaskAsync("New task", "Details", DateTime.UtcNow);

        Assert.NotNull(created);
        Assert.Equal(9, created.Id);
        Assert.Equal("New task", created.Title);
    }

    [Fact]
    public async Task CreateTaskAsync_ReturnsNull_OnFailure()
    {
        var handler = new MockHttpHandler(_ =>
            MockHttpHandler.Empty(HttpStatusCode.BadRequest));

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var created = await service.CreateTaskAsync("", null, null);

        Assert.Null(created);
    }

    [Fact]
    public async Task UpdateTaskAsync_ReturnsUpdatedTask_OnSuccess()
    {
        var handler = new MockHttpHandler(request =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("/api/tasks/5", request.RequestUri!.PathAndQuery);

            return MockHttpHandler.JsonResponse(HttpStatusCode.OK, new TaskItem
            {
                Id = 5,
                Title = "Updated",
                Description = "Changed",
                IsComplete = true
            });
        });

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var updated = await service.UpdateTaskAsync(new TaskItem
        {
            Id = 5,
            Title = "Updated",
            Description = "Changed",
            IsComplete = true
        });

        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Title);
        Assert.True(updated.IsComplete);
    }

    [Fact]
    public async Task UpdateTaskAsync_ReturnsNull_OnFailure()
    {
        var handler = new MockHttpHandler(_ =>
            MockHttpHandler.Empty(HttpStatusCode.NotFound));

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var updated = await service.UpdateTaskAsync(new TaskItem { Id = 99, Title = "Missing" });

        Assert.Null(updated);
    }

    [Fact]
    public async Task DeleteTaskAsync_ReturnsTrue_OnSuccess()
    {
        var handler = new MockHttpHandler(request =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.Equal("/api/tasks/7", request.RequestUri!.PathAndQuery);

            return MockHttpHandler.Empty(HttpStatusCode.NoContent);
        });

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var deleted = await service.DeleteTaskAsync(7);

        Assert.True(deleted);
    }

    [Fact]
    public async Task DeleteTaskAsync_ReturnsFalse_OnFailure()
    {
        var handler = new MockHttpHandler(_ =>
            MockHttpHandler.Empty(HttpStatusCode.NotFound));

        var session = new FakeSessionContext();
        session.Save("token", "alice");
        var service = CreateService(handler, session);

        var deleted = await service.DeleteTaskAsync(404);

        Assert.False(deleted);
    }
}