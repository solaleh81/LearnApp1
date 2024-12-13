using System.Text.Json.Serialization;

namespace LearnApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<CategoryUser>? CategoryUsers { get; set; }

    }
}
