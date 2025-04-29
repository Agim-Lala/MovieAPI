using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Comments;
using MovieAPI.Services;
using System.Security.Claims;

namespace MovieAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

   
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentDTO>> AddComment([FromBody] CreateCommentDTO dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var createdComment = await _commentService.AddCommentAsync(dto, userId);
        return CreatedAtAction(nameof(GetCommentsByMovie), new { movieId = dto.MovieId }, createdComment);
    }

    [HttpPut("{commentId}")]
    [Authorize]
    public async Task<ActionResult<CommentDTO>> UpdateComment(int commentId, [FromBody] UpdateCommentDTO dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedComment = await _commentService.UpdateCommentAsync(commentId, dto, userId);
        if (updatedComment == null) return NotFound("Comment not found or not authorized.");
        return Ok(updatedComment);
    }

    [HttpDelete("{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var deleted = await _commentService.DeleteCommentAsync(commentId, userId);
        if (!deleted) return NotFound("Comment not found or not authorized.");
        return NoContent();
    }

    [HttpGet("movie/{movieId}")]
    public async Task<ActionResult<List<CommentDTO>>> GetCommentsByMovie(int movieId)
    {
        var comments = await _commentService.GetCommentsByMovieAsync(movieId);
        return Ok(comments);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<CommentDTO>>> GetCommentsByUser(int userId)
    {
        var comments = await _commentService.GetCommentsByUserAsync(userId);
        return Ok(comments);
    }

    [HttpPost("{commentId}/like")]
    [Authorize]
    public async Task<IActionResult> LikeComment(int commentId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commentService.LikeCommentAsync(commentId, userId);
        if (!result) return NotFound("Comment not found.");
        return Ok("Comment liked.");
    }

    [HttpPost("{commentId}/dislike")]
    [Authorize]
    public async Task<IActionResult> DislikeComment(int commentId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _commentService.DislikeCommentAsync(commentId, userId);
        if (!result) return NotFound("Comment not found.");
        return Ok("Comment disliked.");
    }
}
