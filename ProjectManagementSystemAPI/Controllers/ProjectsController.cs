using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemAPI.Data;
using ProjectManagementSystemAPI.Models;

namespace ProjectManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            return Ok(await _context.Projects.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project updatedProject)
        {
            if (id != updatedProject.Id)
                return BadRequest();

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            project.Name = updatedProject.Name;
            project.Description = updatedProject.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{projectId}/tasks")]
        public IActionResult GetTasksByProject(int projectId)
        {
            var tasks = _context.Tasks.Where(t => t.ProjectId == projectId).ToList();
            return Ok(tasks);
        }
    }
}
