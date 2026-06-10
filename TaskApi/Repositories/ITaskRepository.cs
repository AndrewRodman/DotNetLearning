using TaskApi.Models;

namespace TaskApi.Repositories;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(int userId, bool? isComplete = null);
    Task<TaskItem?> GetByIdAsync(int userId, int id);
    Task<TaskItem> AddAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(int userId, int id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(int userId, int id);
}