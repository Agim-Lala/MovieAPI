using MovieAPI.Domain.Movies;

namespace MovieAPI.Domain.Users;

public class UserMovieWatch
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
}