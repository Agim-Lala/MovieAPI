using System.ComponentModel.DataAnnotations;
using MovieAPI.Domain.Movies;
using MovieAPI.Domain.Users;

namespace MovieAPI.Domain.Comments;

public class Comment
{
    [Key]
    public int CommentId { get; set; }
    [Required]
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public int? QuotedCommentId { get; set; }
    public Comment? QuotedComment { get; set; }
    public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    

}