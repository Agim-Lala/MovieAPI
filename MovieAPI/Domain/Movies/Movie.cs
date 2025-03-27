using System.ComponentModel.DataAnnotations;
using System.IO;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Directors;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Qualities;

namespace MovieAPI.Domain.Movies
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; }

        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int DirectorId { get; set; }
        public Director Director { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
        
        public ICollection<MovieQuality> MovieQualities { get; set; } = new List<MovieQuality>();
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; 
        
        public string ImagePath { get; set; }


    }
}
