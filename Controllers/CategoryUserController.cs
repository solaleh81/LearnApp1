using LearnApp.Data;
using LearnApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearnApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryUserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryUserController (AppDbContext context)
        {

        _context = context; 
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryUser([FromBody] CategoryUserRequest request)
        {
            if (request == null || request.UserId <= 0 || request.CategoryId <=0)
            {
                return BadRequest("Invalid Data");

            }
            var categoryUser = new CategoryUser
            {
                UserId = request.UserId,
                CategoryId = request.CategoryId
            };
            _context.CategoryUsers.Add(categoryUser);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "CategoryUser reated successfully.",
                Data = categoryUser
            });
        }

        public class CategoryUserRequest
        {
            public int UserId { get; set; }
            public int CategoryId { get; set; }
        }
    }
}
