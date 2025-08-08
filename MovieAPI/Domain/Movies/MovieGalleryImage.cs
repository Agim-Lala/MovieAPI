namespace MovieAPI.Domain.Movies;

public class MovieGalleryImage
{
    public int Id { get; set; }
    public string ImagePath { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; }
}