using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TaskManager.Data;
using TaskManager.DTOs;
using TaskManager.Services;
using TaskManager.Utility;
using TaskManager.Models;
using System.Diagnostics;

namespace task_manager_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JWTService _jwtService;

        public AuthController(ApplicationDbContext context, JWTService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
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

        // REGISTER USER ENDPOINT
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            Debug.WriteLine(request);
            // CHECK IF EMAIL IS ALREADY REGISTERED
            var isExist = _context.Users.Any(u => u.Email == request.Email);

            // RETURNS 409 CONFLICT IF EMAIL IS ALREADY REGISTERED
            if (isExist) return Conflict("Email is already registered.");

            // CREATE NEW USER
            var user = new User
            {
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password)
            };

            // ADDS NEW USER TO CONTEXT
            _context.Users.Add(user);

            // SAVES CHANGES TO DATABASE
            await _context.SaveChangesAsync();

            // GENERATES JWT TOKEN
            var token = _jwtService.GenerateToken(user);

            // RETURNS 201 USER REGISTERED SUCCESSFULLY
            return CreatedAtAction(
                nameof(GetUserById),
                new { Id = user.Id },
                new RegisterResponse(user.Id, user.Email, token)
            );
        }
    }
}
