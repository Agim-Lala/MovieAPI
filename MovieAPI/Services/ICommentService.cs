using MovieAPI.Domain.Comments;
using MovieAPI.Enums;

namespace MovieAPI.Services;

public interface ICommentService
{
    Task<(List<CommentDTO> Comments, int TotalCount)> GetSortedCommentsAsync(CommentSortOption sortBy, bool ascending = true, int page = 1, int pageSize = 10);   
    Task<CommentDTO> AddCommentAsync(CreateCommentDTO dto, int userId);
    Task<bool> DeleteCommentAsync(int commentId, int userId);
    Task<List<CommentDTO>> GetCommentsByUserAsync(int userId);
    Task<List<CommentDTO>> GetCommentsByMovieAsync(int movieId);
    Task<bool> LikeCommentAsync(int commentId, int userId);
    Task<bool> DislikeCommentAsync(int commentId, int userId);
    Task<CommentDTO?> UpdateCommentAsync(int commentId, UpdateCommentDTO dto, int userId);
}