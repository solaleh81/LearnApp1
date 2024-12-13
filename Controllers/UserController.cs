using LearnApp.Data;
using LearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Name, email, and password are required.");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists.");
            }

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful", userId = user.Id });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}

