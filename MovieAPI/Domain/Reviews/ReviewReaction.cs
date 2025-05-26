using MovieAPI.Domain.Users;

namespace MovieAPI.Domain.Reviews;

public class ReviewReaction
{
    public int ReviewReactionId { get; set; }
    public int ReviewId { get; set; }
    public  Review Review {get;set;}
    public int UserId { get; set; }
    public  User User { get; set;}
    public bool IsLike { get; set; }
    public DateTime ReactedAt { get; set; }
    
}