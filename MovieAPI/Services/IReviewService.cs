using MovieAPI.Domain.Reviews;
using MovieAPI.Enums;

namespace MovieAPI.Services;

public interface IReviewService
{
    Task<ReviewDTO> AddReviewAsync(CreateReviewDTO dto, int userId);

    Task<List<ReviewDTO>> GetReviewsByMovieAsync(int movieId);
    Task<List<ReviewDTO>> GetReviewsByUserAsync(int userId);
    Task<double> GetAverageRatingAsync(int movieId);

    Task<(List<ReviewDTO> Reviews, int TotalCount)> GetSortedReviewsAsync(ReviewSortOption sortBy,bool ascending = true, int page = 1, int pageSize = 10);
    
    Task<bool>ReactToReviewAsync(int reviewId, int userId, bool isLike);

    Task<ReviewDTO> GetReviewByIdAsync(int reviewID);
}