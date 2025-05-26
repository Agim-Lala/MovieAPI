using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Reviews;
using MovieAPI.Enums;

namespace MovieAPI.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    public ReviewService(ApplicationDbContext context) => _context = context;


    public async Task<(List<ReviewDTO> Reviews, int TotalCount)> GetSortedReviewsAsync(ReviewSortOption sortBy,
        bool ascending = true, int page = 1, int pageSize = 10)
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
        int totalCount = await query.CountAsync();
        
        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews.Select(MapToDto).ToList(), totalCount);
    }
    
    
    
    
    public async Task<ReviewDTO> AddReviewAsync(CreateReviewDTO dto, int userId)
    {
        var review = new Review {
            MovieId   = dto.MovieId,
            UserId    = userId,
            Text      = dto.Text,
            Rating    = dto.Rating,
            CreatedAt = DateTime.UtcNow
        };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(); // Save the review first

        var movie = await _context.Movies.FindAsync(dto.MovieId);
        if (movie == null) throw new Exception("Movie not found");

        movie.AverageRating = await _context.Reviews
            .Where(r => r.MovieId == dto.MovieId)
            .AverageAsync(r => r.Rating); 
      


        await _context.SaveChangesAsync(); 

        
        var user = await _context.Users.FindAsync(userId);
        review.User = user;


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

    public async Task<bool> ReactToReviewAsync(int reviewId, int userId, bool isLike)
    {
        var review = await _context.Reviews
            .Include(r => r.Reactions)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        
        if(review == null) return false;
        
        var existingReaction =  review.Reactions.FirstOrDefault(r => r.UserId == userId);

        if (existingReaction != null)
        {
            if (existingReaction.IsLike == isLike)
            {
                _context.ReviewReactions.Remove(existingReaction);
            }
            else
            {
                existingReaction.IsLike = isLike;
                existingReaction.ReactedAt = DateTime.UtcNow;
            }
        }
        else
        {
            _context.ReviewReactions.Add(new ReviewReaction
            {
                ReviewId = reviewId,
                UserId = userId,
                IsLike = isLike
            });
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ReviewDTO> GetReviewByIdAsync(int reviewID)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r =>r.Movie)
            .Include(r => r.Reactions)
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


    private ReviewDTO MapToDto(Review review)
    {
        int likeCount = review.Reactions?.Count(r => r.IsLike) ?? 0;
        int dislikeCount = review.Reactions?.Count(r => !r.IsLike) ?? 0;
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
            LikeDislikeText = $"{likeCount}/{dislikeCount}"

            
            
        };
    }
}