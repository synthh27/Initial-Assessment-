using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TaskManager.Data;
using TaskManager.DTOs;

namespace task_manager_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET USER BY ID END POINT
        [HttpGet("user{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            // CHECK IF USER EXISTS
            var user = await _context.Users.FindAsync(id);

            // RETURNS 404 IF USER DOES NOT EXIST
            if (user is null) return NotFound("User does not exist.");

            // RETURNS 200 OK IF USER EXISTS
            return Ok(user);
        }
    }
}
