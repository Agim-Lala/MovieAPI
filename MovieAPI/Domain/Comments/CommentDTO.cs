namespace MovieAPI.Domain.Comments;

public class CommentDTO
{
    public int CommentId { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int LikesCount { get; set; }

    public int DislikesCount { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public int MovieId { get; set; }

    public int? ParentCommentId { get; set; }

    public int? QuotedCommentId { get; set; }

    public string? QuotedText { get; set; } // Add this line

    public CommentDTO? QuotedComment { get; set; } // Add this line

    public List<CommentDTO> Replies { get; set; } = new();
}