namespace MovieAPI.Domain.Qualities;

public class MovieQuality
{
    public int MovieId { get; set; }
    public Movies.Movie Movie { get; set; }

    public int QualityId { get; set; }
    public Quality Quality { get; set; }
}