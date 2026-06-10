using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskApi.Models;
using TaskApi.Repositories;

namespace TaskApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskItem>>> GetAll([FromQuery] bool? isComplete)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var tasks = await repository.GetAllAsync(userId.Value, isComplete);
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetById(int id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var task = await repository.GetByIdAsync(userId.Value, id);
        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(CreateTaskRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var task = new TaskItem
        {
            UserId = userId.Value,
            Title = request.Title,
            Description = request.Description
        };

        var created = await repository.AddAsync(task);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItem>> Update(int id, UpdateTaskRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var updated = await repository.UpdateAsync(userId.Value, id, request);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await repository.DeleteAsync(userId.Value, id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private int? GetUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idClaim, out var userId) ? userId : null;
    }
}