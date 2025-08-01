namespace MovieAPI.Domain.Movies;

public class UpdateMovieDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }
    public int RunningTime { get; set; }
    public int Age { get; set; }
    public string Country { get; set; }
    public List<int> GenreIds { get; set; } = new List<int>();
    public List<int> CategoryIds { get; set; } = new List<int>();
    public List<int> QualityIds { get; set; } = new List<int>();
    public List<int> ActorIds { get; set; } = new List <int>();
    public int DirectorId { get; set; }
    public string Link { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public IFormFile? CoverImage { get; set; }
    public IFormFile? VideoFile { get; set; }
}