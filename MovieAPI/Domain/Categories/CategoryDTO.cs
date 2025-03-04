using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Categories
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
}
