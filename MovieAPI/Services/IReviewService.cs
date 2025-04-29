using MovieAPI.Domain.Reviews;

namespace MovieAPI.Services;

public interface IReviewService
{
    Task<ReviewDTO> AddReviewAsync(CreateReviewDTO dto, int userId);
    Task<List<ReviewDTO>> GetReviewsByMovieAsync(int movieId);
    Task<List<ReviewDTO>> GetReviewsByUserAsync(int userId);
    Task<double> GetAverageRatingAsync(int movieId);
}