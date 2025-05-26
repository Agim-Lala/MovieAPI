using System.ComponentModel.DataAnnotations;
using MovieAPI.Domain.Movies;
using MovieAPI.Domain.Users;

namespace MovieAPI.Domain.Reviews;

public class Review
{
    [Key]
    public int ReviewId { get; set; }

    [Required] public string Text { get; set; } = "";
    
    [Range(0, 10)] public double Rating { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt  { get; set; } 
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    
    public ICollection<ReviewReaction>Reactions { get; set; } = new List<ReviewReaction>();
}