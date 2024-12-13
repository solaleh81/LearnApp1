using LearnApp.Data;
using LearnApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace LearnApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IConnectionMultiplexer _redis;

        public CategoryController(AppDbContext context, IConnectionMultiplexer redis)
        {

            _context = context;
            _redis = redis;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrEmpty(categoryDto.Name))
            {
                return BadRequest("Category name is required");
            }

            var category = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Category), new { id = category.Id }, category);
        }



        public class AddUserToCategoryRequest
        {
            public string CategoryName { get; set; } 
            public int UserId { get; set; } 
            public int UserIdToAdd { get; set; }
        }

        [HttpPost("add-user-to-category")]
        public async Task<IActionResult> AddUserToCategory([FromBody] AddUserToCategoryRequest request)
        {
            var mainUser = await _context.Users.FindAsync(request.UserId);
            if (mainUser == null)
            {
                return NotFound($"User with ID {request.UserId} not found.");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == request.CategoryName);
            if (category == null)
            {
                return NotFound($"Category with name {request.CategoryName} not found.");
            }

            var userToAdd = await _context.Users.FindAsync(request.UserIdToAdd);
            if (userToAdd == null)
            {
                return NotFound($"User with ID {request.UserIdToAdd} not found.");
            }

            var db = _redis.GetDatabase();
            var key = $"user:{mainUser.Id}:category:{category.Name}:users";

            var isMember = await db.SetContainsAsync(key, request.UserIdToAdd);
            if (isMember)
            {
                return BadRequest($"User {request.UserIdToAdd} is already in category {category.Name}.");
            }

            await db.SetAddAsync(key, request.UserIdToAdd);

            return Ok($"User {request.UserIdToAdd} successfully added to category {category.Name}.");
        }
        public class GetUsersRequest
        {
            public int userId { get; set; }
            public string categoryName { get; set; }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsersFromCategory([FromBody] GetUsersRequest request)
        {
            var userExists = await _context.Users.FindAsync(request.userId);
            if (userExists == null)
            {
                return NotFound($"User with ID {request.userId} not found.");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == request.categoryName);
            if (category == null)
            {
                return NotFound($"Category with name {request.categoryName} not found.");
            }

            var db = _redis.GetDatabase();
            var key = $"user:{userExists.Id}:category:{category.Name}:users";

            var userIds = await db.SetMembersAsync(key);

            if (userIds.Length == 0)
            {
                return NotFound($"No users found in category {request.categoryName}.");
            }

            var users = new List<User>();
            foreach (var userId in userIds)
            {
                int userIdInt = Convert.ToInt32(userId);

                Console.WriteLine($"Checking user with ID: {userIdInt}");

                var user = await _context.Users.FindAsync(userIdInt);
                if (user == null)
                {
                    Console.WriteLine($"User with ID {userIdInt} not found.");
                }
                else
                {
                    Console.WriteLine($"User with ID {userIdInt} found: {user.Name}");
                    users.Add(user);
                }
            }

            if (users.Count == 0)
            {
                return NotFound($"No users found in category {request.categoryName}.");
            }

            return Ok(users);
        }




        [HttpDelete("{categoryId}/remove-user")]
        public async Task<IActionResult> RemoveUserFromCategory(int categoryId, [FromBody] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("User ID is required and must be a positive number.");

            }
            var categoryExists = await _context.Categories.FindAsync(categoryId);
            if (categoryExists == null)
            {
                return NotFound($"This Category with categoryId: {categoryId} does not exist");
            }

            var userExists = await _context.Users.FindAsync(userId);
            if (userExists == null)
            {
                return NotFound($"User with ID {userId} not found.");
            }
            var db = _redis.GetDatabase();
            var key = $"category:{categoryId}:users";

            var isMember = await db.SetContainsAsync(key, userId);

            if (!isMember)
            {
                return NotFound($"User {userId} is not part of category {categoryId}.");
            }

            await db.SetRemoveAsync(key, userId);
            return Ok($"User {userId} removed from category {categoryId}.");

        }
    }
}
    

