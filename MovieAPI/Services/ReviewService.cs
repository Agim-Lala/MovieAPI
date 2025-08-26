using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Reviews;
using MovieAPI.Enums;

namespace MovieAPI.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    public ReviewService(ApplicationDbContext context) => _context = context;


    public async Task<int> GetNewReviewsCountAsync()
    {
        var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return await _context.Reviews.CountAsync(r => r.CreatedAt >= firstDayOfMonth);
    }
    
    public async Task<(List<ReviewDTO> Reviews, int TotalCount)> GetSortedReviewsAsync(ReviewSortOption sortBy,
        bool ascending = true, int page = 1, int pageSize = 10, string search = null)
    {
        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Movie)
            .AsQueryable();
        
       

        query = (sortBy, ascending) switch
        {
            (ReviewSortOption.Id, true) => query.OrderBy(r => r.ReviewId),
            (ReviewSortOption.Id, false) => query.OrderByDescending(r => r.ReviewId),
            (ReviewSortOption.Title, true) => query.OrderBy(r => r.Movie.Title),
            (ReviewSortOption.Title, false) => query.OrderByDescending(r => r.Movie.Title),
            (ReviewSortOption.Author, true) => query.OrderBy(r => r.User.FirstName + " " + r.User.LastName),
            (ReviewSortOption.Author, false) => query.OrderByDescending(r => r.User.FirstName + " " + r.User.LastName),
            (ReviewSortOption.CreatedAt, true) => query.OrderBy(r => r.CreatedAt),
            (ReviewSortOption.CreatedAt, false) => query.OrderByDescending(r => r.CreatedAt),
            (ReviewSortOption.Rating, true) => query.OrderBy(r => r.Rating),
            (ReviewSortOption.Rating, false) => query.OrderByDescending(r => r.Rating),


        };
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowered = search.ToLower();
            query = query.Where(r =>
                r.Text.ToLower().Contains(lowered) ||
                r.Movie.Title.ToLower().Contains(lowered)
                    
            );
        }
        int totalCount = await query.CountAsync();
        
        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews.Select(MapToDto).ToList(), totalCount);
    }
    
    
    
    
    public async Task<ReviewDTO> AddReviewAsync(CreateReviewDTO dto, int userId)
    {
        var existingReview = await _context.Reviews
            .FirstOrDefaultAsync(r => r.MovieId == dto.MovieId && r.UserId == userId);

        if (existingReview != null)
        {
            throw new InvalidOperationException("User has already submitted a review for this movie.");
        }

        var review = new Review {
            MovieId   = dto.MovieId,
            UserId    = userId,
            Text      = dto.Text,
            Rating    = dto.Rating,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

       
        await _context.SaveChangesAsync();
        await CalculateAverageRatingAsync(dto.MovieId);
        review.User = await _context.Users.FindAsync(userId);

        return MapToDto(review);
    }

    public async Task<ReviewDTO> UpdateReviewAsync(int reviewId, int userId, UpdateReviewDTO dto)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null) return null;

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("You cannot edit this review.");

        review.Text = dto.Text;
        review.Rating = dto.Rating;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        await CalculateAverageRatingAsync(review.MovieId);

        review.User = await _context.Users.FindAsync(userId);
        return MapToDto(review);
    }

    public async Task<List<ReviewDTO>> GetReviewsByMovieAsync(int movieId) {
        
       var reviews = await _context.Reviews
            .Where(r => r.MovieId == movieId)
            .Include(r => r.User) 
            .Include(r => r.Movie)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
       
       return reviews.Select(r => MapToDto(r)).ToList();
    }

    public async Task<ReviewDTO> GetReviewById(int reviewId)
    {
        var review = await _context.Reviews
            .Where(r => r.ReviewId == reviewId)
            .Include(r => r.User)
            .Include(r => r.Movie)
            .FirstOrDefaultAsync();

        if (review == null) return null;

        return MapToDto(review);
    }

    public async Task<List<ReviewDTO>> GetReviewsByUserAsync(int userId)
    {
        var reviews =  await _context.Reviews
            .Where(r => r.UserId == userId)
            .Include(r => r.Movie) 
            .Include(r => r.User) 
            .OrderByDescending(r => r.CreatedAt) 
            .ToListAsync();
       
        return reviews.Select(r => MapToDto(r)).ToList();
    }
    

    public async Task<ReviewDTO> GetReviewByIdAsync(int reviewID)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r =>r.Movie)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewID);
        
        if (review == null) return null;
        
        return MapToDto(review);
    }

    public async Task<double> GetAverageRatingAsync(int movieId) {
        var ratings = await _context.Reviews
            .Where(r => r.MovieId == movieId)
            .ToListAsync(); 

        if (!ratings.Any()) return 0.0; 

        
        double average = ratings.Average(r => r.Rating);

        return Math.Round(average, 2); 

       
        
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null) return false;

        int movieId = review.MovieId;
        
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        await CalculateAverageRatingAsync(movieId);

        return true;
    }


    private ReviewDTO MapToDto(Review review)
    {
        return new ReviewDTO
        {
            ReviewId = review.ReviewId,
            MovieId = review.MovieId,
            UserId = review.UserId,
            Author = review.User != null ? $"{review.User.FirstName} {review.User.LastName}" : "N/A",
            Username = review.User?.Username ?? "Unknown",
            MovieTitle = review.Movie?.Title ?? "Unknown",
            Text = review.Text,
            Rating = review.Rating,
            CreatedAt = review.CreatedAt,
        };
    }

    private async Task CalculateAverageRatingAsync(int movieId)
    {
        var movie = await _context.Movies.FindAsync(movieId);
        if (movie ==  null) return;

        var hasReviews = await _context.Reviews.AnyAsync(r => r.MovieId == movieId);

        double average = 0;
        if (hasReviews)
        {
            average = await _context.Reviews
                .Where(r => r.MovieId == movieId)
                .AverageAsync(r => r.Rating);
        }

        movie.AverageRating = Math.Round(average,1);
        await _context.SaveChangesAsync();
    }
}