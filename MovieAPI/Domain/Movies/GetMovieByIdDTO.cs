using MovieAPI.Domain.Actors;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Directors;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Qualities;

namespace MovieAPI.Domain.Movies;

public class GetMovieByIdDTO
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public int ReleaseYear { get; set; }
    public string Description { get; set; }

    public DirectorDTO Director { get; set; }

    public List<GenreDTO> Genres { get; set; } = new();
    public List<CategoryDTO> Categories { get; set; } = new();
    public List<QualityDTO> Qualities { get; set; } = new();
    public List<ActorDTO> Actors { get; set; } = new();

    public DateTime AddedAt { get; set; }
    public string ImagePath { get; set; }
    public string VideoPath { get; set; }
    public string Link { get; set; }
    public double AverageRating { get; set; }
    public int RunningTime { get; set; }
    public int Age { get; set; }
    public string Country { get; set; }
}