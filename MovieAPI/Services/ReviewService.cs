using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Reviews;

namespace MovieAPI.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;
    public ReviewService(ApplicationDbContext context) => _context = context;


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

        
        return new ReviewDTO {
            ReviewId    = review.ReviewId,
            MovieId     = review.MovieId,     
            UserId      = review.UserId,      
            Username    = user?.Username ?? "Unknown", 
            Text        = review.Text,
            Rating      = review.Rating,
            CreatedAt   = review.CreatedAt
          
        };
    }

    public async Task<List<ReviewDTO>> GetReviewsByMovieAsync(int movieId) {
        
        return await _context.Reviews
            .Where(r => r.MovieId == movieId)
            .Include(r => r.User) 
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDTO {
                ReviewId = r.ReviewId,
                Text = r.Text,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                Username = r.User != null ? r.User.Username : "Unknown", 
                MovieId = r.MovieId, 
                UserId = r.UserId    
                
            })
            .ToListAsync();
    }

    public async Task<List<ReviewDTO>> GetReviewsByUserAsync(int userId)
    {
        return await _context.Reviews
            .Where(r => r.UserId == userId)
            .Include(r => r.Movie) 
            .Include(r => r.User) 
            .OrderByDescending(r => r.CreatedAt) 
            .Select(r => new ReviewDTO
            {
                ReviewId = r.ReviewId,
                Text = r.Text,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                Username = r.User != null ? r.User.Username : "Unknown", 
                MovieTitle = r.Movie != null ? r.Movie.Title : "Unknown", 
                MovieId = r.MovieId, 
                UserId = r.UserId    
            })
            .ToListAsync();
    }

    public async Task<double> GetAverageRatingAsync(int movieId) {
        var ratings = await _context.Reviews
            .Where(r => r.MovieId == movieId)
            .ToListAsync(); 

        if (!ratings.Any()) return 0.0; 

        
        double average = ratings.Average(r => r.Rating);

        return Math.Round(average, 2); 

       
        
    }
}