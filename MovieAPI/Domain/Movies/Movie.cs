using System.ComponentModel.DataAnnotations;
using System.IO;
using MovieAPI.Domain.Actors;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Comments;
using MovieAPI.Domain.Directors;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Qualities;
using MovieAPI.Domain.Reviews;

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
        
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>(); 
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; 
        
        public string ImagePath { get; set; }
        
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public double AverageRating { get; set; } 
        
        public bool IsVisible { get; set; } = true;  
        
        public int Views { get; set; } = 0;



    }
}
