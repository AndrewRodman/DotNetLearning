using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TaskApp.Models;

namespace TaskApp.Services;

public class TaskApiService(HttpClient httpClient, ISessionContext session) : ITaskApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        if (auth is not null)
        {
            session.Save(auth.Token, auth.Username);
            SetAuthHeader(auth.Token);
        }

        return auth;
    }

    public async Task<AuthResponse?> RegisterAsync(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/register", new LoginRequest
        {
            Username = username,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        if (auth is not null)
        {
            session.Save(auth.Token, auth.Username);
            SetAuthHeader(auth.Token);
        }

        return auth;
    }

    public async Task<IReadOnlyList<TaskItem>> GetTasksAsync(bool? isComplete = null)
    {
        EnsureAuthHeader();
        
        if (isComplete == null)
        {
            var tasks = await httpClient.GetFromJsonAsync<List<TaskItem>>("api/tasks", JsonOptions);
            return tasks ?? [];
        }
        else
        {
            var tasks = await httpClient.GetFromJsonAsync<List<TaskItem>>($"api/tasks?isComplete={isComplete}", JsonOptions);
            return tasks ?? [];
        }     
    }

    public async Task<TaskItem?> CreateTaskAsync(string title, string? description)
    {
        EnsureAuthHeader();
        var response = await httpClient.PostAsJsonAsync("api/tasks", new CreateTaskRequest
        {
            Title = title,
            Description = description
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskItem>(JsonOptions);
    }

    public async Task<TaskItem?> UpdateTaskAsync(TaskItem task)
    {
        EnsureAuthHeader();
        var response = await httpClient.PutAsJsonAsync($"api/tasks/{task.Id}", new UpdateTaskRequest
        {
            Title = task.Title,
            Description = task.Description,
            IsComplete = task.IsComplete
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskItem>(JsonOptions);
    }
    public async Task<bool> DeleteTaskAsync(int id)
    {
        EnsureAuthHeader();

        var response = await httpClient.DeleteAsync($"api/tasks/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        return true;
    }
    private void EnsureAuthHeader()
    {
        if (!string.IsNullOrWhiteSpace(session.Token))
        {
            SetAuthHeader(session.Token);
        }
    }

    private void SetAuthHeader(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}