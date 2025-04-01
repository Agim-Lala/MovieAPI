using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Movies
{
    public class CreateMovieDTO
    {
        [Required, StringLength(255)]
        public string Title { get; set; }

        [Range(1888, 2100)]
        public int ReleaseYear { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public int DirectorId { get; set; }

        public List<int> GenreIds { get; set; } = new List<int>();
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> QualityIds { get; set; } = new List<int>(); 
        
        public List<int> ActorIds { get; set; } = new List<int>(); 
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow; 
        
        public string ImagePath { get; set; }

    }
}
