using LearnApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearnApp.Models;
namespace LearnApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCategoryUser : ControllerBase
    {
        private readonly AppDbContext _context;

        public GetCategoryUser(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersWithCategories()
        {
            var result = await (from user in _context.Users
                                join categoryUser in _context.CategoryUsers on user.Id equals categoryUser.UserId
                                join category in _context.Categories on categoryUser.CategoryId equals category.Id
                                group new { category.Id, category.Name } by new { user.Id, user.Name } into grouped
                                select new
                                {
                                    UserId = grouped.Key.Id,
                                    UserName = grouped.Key.Name,
                                    Categories = grouped.Select(g => new
                                    {
                                        g.Id,
                                        g.Name
                                    }).ToList()
                                }).ToListAsync();

            return Ok(result);

        }
    }
}