using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemAPI.Data;
using Task = ProjectManagementSystemAPI.Models.Task;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        return Ok(await _context.Tasks.ToListAsync());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, Task updatedTask)
    {
        if (id != updatedTask.Id)
            return BadRequest();

        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        task.Name = updatedTask.Name;
        task.Description = updatedTask.Description;
        task.ProjectId = updatedTask.ProjectId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{projectId}/tasks")]
    public async Task<IActionResult> AddTaskToProject(int projectId, [FromBody] Task task)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return NotFound();

        task.ProjectId = projectId;
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }
}
