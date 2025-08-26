using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Reviews;
using MovieAPI.Enums;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase {
    
    private readonly IReviewService _service;
    public ReviewsController(IReviewService s) => _service = s;
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReviewDTO>> Add([FromBody] CreateReviewDTO dto) {
        var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var created = await _service.AddReviewAsync(dto, uid);
        return CreatedAtAction(nameof(GetByMovie), new { movieId = dto.MovieId }, created);
    }
    [HttpPut("{id}")]
    [Authorize] 
    public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDTO dto)
    {
        var uid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var updated = await _service.UpdateReviewAsync(id, uid, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("new-reviews-count")]
    public async Task<IActionResult> GetNewReviewsCount()
    {
        var count = await _service.GetNewReviewsCountAsync();
        return Ok(new{count});
    }
    
    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<List<ReviewDTO>>> GetByMovie(int movieId) {
        return Ok(await _service.GetReviewsByMovieAsync(movieId));
    }

    [Authorize]
    [HttpGet("movie/{movieId}/average")]
    public async Task<ActionResult<double>> Average(int movieId) {
        return Ok(await _service.GetAverageRatingAsync(movieId));
    }
    
   
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ReviewDTO>>> GetByUser(int userId)
    {
        var list = await _service.GetReviewsByUserAsync(userId);
        return Ok(list);
    }

    [HttpGet("{reviewId}")]
    public async Task<IActionResult> GetReviewById(int reviewId)
    {
        var review = await _service.GetReviewByIdAsync(reviewId);
        return Ok(review);
    }

    [HttpGet("sorted")]
    public async Task<IActionResult> GetSortedReviewsAsync([FromQuery] string sortBy, int page, int pageSize,
        [FromQuery] bool ascending = true,
        [FromQuery] string query = null
        )
    {
        if (!Enum.TryParse<ReviewSortOption>(sortBy, true, out var sortOption))
        {
            return BadRequest($"Invalid sort option: {sortBy}. Valid options are: {string.Join(", ", Enum.GetNames(typeof(ReviewSortOption)))}");
        }

        var (reviews, totalCount) = await _service.GetSortedReviewsAsync(sortOption, ascending, page, pageSize, query);
        return Ok(new {reviews,totalCount});
    }
    
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteReview(int id)
    {
        var deleted = await _service.DeleteReviewAsync(id);

        if (!deleted)
        {
            return NotFound(); 
        }

        return Ok(); 
    }
}