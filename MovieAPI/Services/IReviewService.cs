using MovieAPI.Domain.Reviews;
using MovieAPI.Enums;

namespace MovieAPI.Services;

public interface IReviewService
{
    Task<ReviewDTO> AddReviewAsync(CreateReviewDTO dto, int userId);

    Task<List<ReviewDTO>> GetReviewsByMovieAsync(int movieId);
    Task<List<ReviewDTO>> GetReviewsByUserAsync(int userId);
    Task<double> GetAverageRatingAsync(int movieId);

    Task<(List<ReviewDTO> Reviews, int TotalCount)> GetSortedReviewsAsync(ReviewSortOption sortBy,bool ascending = true, int page = 1, int pageSize = 10, string search = null);
    Task<ReviewDTO> GetReviewByIdAsync(int reviewID);
    Task<int> GetNewReviewsCountAsync();
    Task<bool> DeleteReviewAsync(int id);
    Task<ReviewDTO> UpdateReviewAsync(int reviewId, int userId, UpdateReviewDTO dto);

}