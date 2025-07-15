namespace MovieAPI.Domain.Comments;

public class CreateCommentDTO
{
    public string Text { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public int? ParentCommentId { get; set; }
    public int? QuotedCommentId { get; set; }
}