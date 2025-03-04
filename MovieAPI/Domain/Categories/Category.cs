using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Categories
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }
}
