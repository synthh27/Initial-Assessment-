using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManager.Data;
using TaskManager.DTOs;
using TaskManager.Models;
namespace TaskManager.API
{
    [Authorize]
    [Route("tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // EXTRACT USER ID FROM JWT
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // FETCH TASKS FOR THE SPECIFIED USER
            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            // RETURNS 404 IF NO TASKS FOUND
            if (!tasks.Any()) return NotFound("No task found.");

            // RETURNS 200 USER TASKS
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            // EXTRACT USER ID FROM JWT
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var newTask = new TaskItem
            {
                Title = request.Title,
                IsDone = false, //DEFAULT TO FALSE
                UserId = userId
            };

            // ADD NEW TASK TO DATABASE
            _context.Tasks.Add(newTask);
            // SAVE CHANGES
            await _context.SaveChangesAsync();

            // RETURNS 201 WITH THE NEW TASK
            return CreatedAtAction(nameof(Get), new { id = newTask.Id }, newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem updated)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            task.Title = updated.Title;
            task.IsDone = updated.IsDone;
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
