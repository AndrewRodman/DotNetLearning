using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Repositories;

public class TaskRepository(AppDbContext db) : ITaskRepository
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(bool? isComplete = null)
    {
        var query = db.Tasks.AsQueryable();

        if (isComplete is not null)
        {
            query = query.Where(t => t.IsComplete == isComplete);
        }

        return await query
            .OrderBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await db.Tasks.FindAsync(id);
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(int id, UpdateTaskRequest request)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null)
        {
            return null;
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.IsComplete = request.IsComplete;

        await db.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null)
        {
            return false;
        }

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return true;
    }
}