using MovieAPI.Domain.Comments;

namespace MovieAPI.Services;

public interface ICommentService
{
    Task<CommentDTO> AddCommentAsync(CreateCommentDTO dto, int userId);
    Task<bool> DeleteCommentAsync(int commentId, int userId);
    Task<List<CommentDTO>> GetCommentsByUserAsync(int userId);
    Task<List<CommentDTO>> GetCommentsByMovieAsync(int movieId);
    Task<bool> LikeCommentAsync(int commentId, int userId);
    Task<bool> DislikeCommentAsync(int commentId, int userId);
    Task<CommentDTO?> UpdateCommentAsync(int commentId, UpdateCommentDTO dto, int userId);
}