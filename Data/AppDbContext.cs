using LearnApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnApp.Data
{
    public class AppDbContext :DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User>  Users { get; set; }
        public DbSet<CategoryUser> CategoryUsers { get; set; }
        public DbSet<Category> Categories { get; set; }



    }
}
