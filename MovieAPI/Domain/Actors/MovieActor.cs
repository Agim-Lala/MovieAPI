using MovieAPI.Domain.Movies;

namespace MovieAPI.Domain.Actors;

public class MovieActor
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    public int ActorId { get; set; }
    public Actors.Actor Actor { get; set; }
}