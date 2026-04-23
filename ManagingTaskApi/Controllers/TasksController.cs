
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskManagementApi.Data;
using TaskManagementApi.Models;

namespace ManagingTaskApi.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
[EnableRateLimiting("taskPolicy")]
public class TasksController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetAll()
    {
        return Ok(TaskStore.Tasks);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetById(int id)
    {
        var task = TaskStore.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            return NotFound(new { message = "Task not found." });

        return Ok(task);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public IActionResult Create([FromBody] TaskItem task)
    {
        var newId = TaskStore.Tasks.Count == 0 ? 1 : TaskStore.Tasks.Max(t => t.Id) + 1;

        task.Id = newId;
        TaskStore.Tasks.Add(task);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, [FromBody] TaskItem updatedTask)
    {
        var task = TaskStore.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            return NotFound(new { message = "Task not found." });

        task.Title = updatedTask.Title;
        task.IsCompleted = updatedTask.IsCompleted;

        return Ok(task);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var task = TaskStore.Tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
            return NotFound(new { message = "Task not found." });

        TaskStore.Tasks.Remove(task);
        return Ok(new { message = "Task deleted successfully." });
    }
}