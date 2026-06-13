using TaskApp.Models;

namespace TaskApp.Services;

public interface ITaskApiService
{
    Task<AuthResponse?> LoginAsync(string username, string password);
    Task<AuthResponse?> RegisterAsync(string username, string password);
    Task<IReadOnlyList<TaskItem>> GetTasksAsync(bool? isComplete = null);
    Task<TaskItem?> CreateTaskAsync(string title, string? description, DateTime? dueDate);
    Task<TaskItem?> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(int id);
}