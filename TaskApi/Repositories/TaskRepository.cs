using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Repositories;

public class TaskRepository(AppDbContext db) : ITaskRepository
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(int userId, bool? isComplete = null)
    {
        var query = db.Tasks.Where(t => t.UserId == userId);

        if (isComplete is not null)
        {
            query = query.Where(t => t.IsComplete == isComplete);
        }

        return await query
            .OrderBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int userId, int id)
    {
        return await db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(int userId, int id, UpdateTaskRequest request)
    {
        var task = await GetByIdAsync(userId, id);
        if (task is null)
        {
            return null;
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.IsComplete = request.IsComplete;
        task.DueDate = request.DueDate;

        await db.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var task = await GetByIdAsync(userId, id);
        if (task is null)
        {
            return false;
        }

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return true;
    }
}