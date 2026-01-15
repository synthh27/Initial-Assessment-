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

            // CREATE NEW TASK ITEM
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
        {
            // GET USER ID FROM JWT
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            // RETURNS 404 IF TASK NOT FOUND
            if (task == null) return NotFound();

            // UPDATE TASK PROPERTIES IF THEY ARE PROVIDED
            if (request.Title != null) task.Title = request.Title;
            if (request.IsDone.HasValue) task.IsDone = request.IsDone.Value;

            // SAVE CHANGES TO DATABASE
            await _context.SaveChangesAsync();

            // RETURNS 200 FOR SUCCESS UPDATE
            return Ok("Tasks updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // FIND TASK BY ID
            var task = await _context.Tasks.FindAsync(id);

            // RETURN 404 IF TASK NOT FOUND
            if (task is null) return NotFound();

            // DELETE TASK FROM DATABASE
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok("Task deleted successfully");
        }
    }
}
