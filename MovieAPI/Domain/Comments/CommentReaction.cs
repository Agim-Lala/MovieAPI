using MovieAPI.Domain.Users;

namespace MovieAPI.Domain.Comments;

public class CommentReaction
{
    public int CommentReactionId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
    public bool IsLike { get; set; }
}