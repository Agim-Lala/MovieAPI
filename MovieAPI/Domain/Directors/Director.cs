using MovieAPI.Domain.Movies;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MovieAPI.Domain.Directors
{
    public class Director
    {
        [Key]
        public int DirectorId { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();


    }

   
}
