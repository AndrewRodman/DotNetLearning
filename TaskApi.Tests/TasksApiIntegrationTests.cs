using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskApi.Models;

namespace TaskApi.Tests;

public class TasksApiIntegrationTests(TaskApiWebApplicationFactory factory) : IClassFixture<TaskApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("Healthy", body);
    }

    [Fact]
    public async Task GetTasks_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_Login_CreateTask_GetTasks_WorksEndToEnd()
    {
        var username = $"user_{Guid.NewGuid():N}";
        const string password = "Password123";

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest
        {
            Username = username,
            Password = password
        });
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });
        loginResponse.EnsureSuccessStatusCode();

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Integration test task",
            Description = "Full HTTP pipeline",
            DueDate = DateTime.UtcNow
        });
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

        var getResponse = await _client.GetFromJsonAsync<List<TaskItem>>("/api/tasks");
        Assert.NotNull(getResponse);
        Assert.Contains(getResponse, t => t.Title == "Integration test task");
    }

    [Fact]
    public async Task DeleteTask_RemovesTaskFromList()
    {
        var username = $"user_{Guid.NewGuid():N}";
        const string password = "Password123";

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest
        {
            Username = username,
            Password = password
        });
        registerResponse.EnsureSuccessStatusCode();

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });
        loginResponse.EnsureSuccessStatusCode();

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Integration delete test task",
            Description = "Full HTTP pipeline testing delete"
        });
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var tasks = await _client.GetFromJsonAsync<List<TaskItem>>("/api/tasks");
        Assert.NotNull(tasks);
        Assert.DoesNotContain(tasks, t => t.Id == created.Id);
    }

    [Fact]
    public async Task Users_OnlySeeTheirOwnTasks()
    {
        var userA = $"user_a_{Guid.NewGuid():N}";
        var userB = $"user_b_{Guid.NewGuid():N}";
        const string password = "Password123";

        async Task<string> RegisterAndLoginAsync(string username)
        {
            await _client.PostAsJsonAsync("/api/auth/register", new LoginRequest
            {
                Username = username,
                Password = password
            });

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Username = username,
                Password = password
            });
            loginResponse.EnsureSuccessStatusCode();

            var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(auth);
            return auth.Token;
        }

        var tokenA = await RegisterAndLoginAsync(userA);
        var tokenB = await RegisterAndLoginAsync(userB);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "User A private task",
            Description = "Should not leak"
        });
        createResponse.EnsureSuccessStatusCode();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var userBTasks = await _client.GetFromJsonAsync<List<TaskItem>>("/api/tasks");

        Assert.NotNull(userBTasks);
        Assert.DoesNotContain(userBTasks, t => t.Title == "User A private task");
    }
}