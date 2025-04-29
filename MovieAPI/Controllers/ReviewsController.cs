using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Reviews;
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

    [Authorize]
    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<List<ReviewDTO>>> GetByMovie(int movieId) {
        return Ok(await _service.GetReviewsByMovieAsync(movieId));
    }

    [Authorize]
    [HttpGet("movie/{movieId}/average")]
    public async Task<ActionResult<double>> Average(int movieId) {
        return Ok(await _service.GetAverageRatingAsync(movieId));
    }
    
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ReviewDTO>>> GetByUser(int userId)
    {
        var list = await _service.GetReviewsByUserAsync(userId);
        return Ok(list);
    }
    
}