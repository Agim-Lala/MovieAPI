using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Actors;

public class Actor
{
    [Key]
    public int ActorId { get; set; }

    [Required, StringLength(255)]
    public string ActorName { get; set; }

    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}