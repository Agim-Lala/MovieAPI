using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Comments;

namespace MovieAPI.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;

    public CommentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CommentDTO> AddCommentAsync(CreateCommentDTO dto, int userId)
    {
        var comment = new Comment
        {
            Text = dto.Text, 
            MovieId = dto.MovieId,
            UserId = userId,
            ParentCommentId = dto.ParentCommentId,
            QuotedCommentId = dto.QuotedCommentId,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return await BuildCommentDTOAsync(comment);
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);
        if (comment == null) return false;

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CommentDTO>> GetCommentsByUserAsync(int userId)
    {
        var comments = await _context.Comments
            .Where(c => c.UserId == userId && c.ParentCommentId == null)
            .Include(c => c.Replies)
            .ToListAsync();

        return (await Task.WhenAll(comments.Select(BuildCommentDTOAsync))).ToList();
    }

    public async Task<List<CommentDTO>> GetCommentsByMovieAsync(int movieId)
    {
        var comments = await _context.Comments
            .Where(c => c.MovieId == movieId && c.ParentCommentId == null)
            .Include(c => c.User) // Include the User for direct access to Username
            .Include(c => c.Replies)
            .ThenInclude(r => r.User).Include(comment => comment.LikedByUsers)
            .Include(comment => comment.DislikedByUsers).Include(comment => comment.Replies)
            .ThenInclude(comment => comment.LikedByUsers).Include(comment => comment.Replies)
            .ThenInclude(comment => comment.DislikedByUsers) // Include User for replies as well
            .ToListAsync();

        return comments.Select(comment => new CommentDTO
        {
            CommentId = comment.CommentId,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            LikesCount = comment.LikedByUsers?.Count ?? 0,
            DislikesCount = comment.DislikedByUsers?.Count ?? 0,
            MovieId = comment.MovieId,
            UserId = comment.UserId,
            Username = comment.User?.Username ?? "Unknown",
            ParentCommentId = comment.ParentCommentId,
            QuotedCommentId = comment.QuotedCommentId,
            // You'd need another query or include for QuotedText if needed frequently
            Replies = comment.Replies.Select(reply => new CommentDTO
            {
                CommentId = reply.CommentId,
                Text = reply.Text,
                CreatedAt = reply.CreatedAt,
                UpdatedAt = reply.UpdatedAt,
                LikesCount = reply.LikedByUsers?.Count ?? 0,
                DislikesCount = reply.DislikedByUsers?.Count ?? 0,
                MovieId = reply.MovieId,
                UserId = reply.UserId,
                Username = reply.User?.Username ?? "Unknown",
                ParentCommentId = reply.ParentCommentId,
                QuotedCommentId = reply.QuotedCommentId,
                Replies = new List<CommentDTO>() 
            }).ToList()
        }).ToList();
    }

    public async Task<bool> LikeCommentAsync(int commentId, int userId)
    {
        var comment = await _context.Comments
            .Include(c => c.LikedByUsers)
            .Include(c => c.DislikedByUsers)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        if (comment == null) return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        comment.DislikedByUsers.Remove(user);
        if (!comment.LikedByUsers.Contains(user))
            comment.LikedByUsers.Add(user);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DislikeCommentAsync(int commentId, int userId)
    {
        var comment = await _context.Comments
            .Include(c => c.LikedByUsers)
            .Include(c => c.DislikedByUsers)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        if (comment == null) return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        comment.LikedByUsers.Remove(user);
        if (!comment.DislikedByUsers.Contains(user))
            comment.DislikedByUsers.Add(user);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CommentDTO?> UpdateCommentAsync(int commentId, UpdateCommentDTO dto, int userId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);
        if (comment == null) return null;

        comment.Text = dto.Text;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await BuildCommentDTOAsync(comment);
    }

   private async Task<CommentDTO> BuildCommentDTOAsync(Comment comment)
{
    var replies = await _context.Comments
        .Where(r => r.ParentCommentId == comment.CommentId)
        .ToListAsync();

    Comment quotedCommentEntity = null;
    if (comment.QuotedCommentId.HasValue)
    {
        quotedCommentEntity = await _context.Comments.FindAsync(comment.QuotedCommentId);
    }

    CommentDTO quotedCommentDTO = null;
    if (quotedCommentEntity != null)
    {
        quotedCommentDTO = new CommentDTO
        {
            CommentId = quotedCommentEntity.CommentId,
            Text = quotedCommentEntity.Text,
            Username = await _context.Users
                .Where(u => u.Id == quotedCommentEntity.UserId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync() ?? "Unknown",
            CreatedAt = quotedCommentEntity.CreatedAt,
            // You can include other relevant properties if needed
        };
    }

    string? quotedText = comment.QuotedCommentId.HasValue
        ? await _context.Comments
            .Where(q => q.CommentId == comment.QuotedCommentId)
            .Select(q => q.Text)
            .FirstOrDefaultAsync()
        : null;

    return new CommentDTO
    {
        CommentId = comment.CommentId,
        Text = comment.Text,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt,
        LikesCount = comment.LikedByUsers?.Count ?? 0,
        DislikesCount = comment.DislikedByUsers?.Count ?? 0,
        MovieId = comment.MovieId,
        UserId = comment.UserId,
        Username = await _context.Users
            .Where(u => u.Id == comment.UserId)
            .Select(u => u.Username)
            .FirstOrDefaultAsync() ?? "Unknown",
        ParentCommentId = comment.ParentCommentId,
        QuotedCommentId = comment.QuotedCommentId,
        QuotedText = quotedText, // Using the locally defined quotedText
        QuotedComment = quotedCommentDTO, // Using the created quotedCommentDTO
        Replies = (await Task.WhenAll(replies.Select(BuildCommentDTOAsync))).ToList()
    };
}

}



