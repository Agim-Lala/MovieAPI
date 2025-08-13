using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Comments;
using MovieAPI.Enums;

namespace MovieAPI.Services;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;

    public CommentService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<(List<CommentDTO> Comments, int TotalCount)> GetSortedCommentsAsync(
        CommentSortOption sortBy,
        bool ascending = true,
        int page = 1,
        int pageSize = 10)
    {
        var query = _context.Comments
            .Include(c => c.User)
            .Include(c => c.Movie)
            .Include(c => c.Replies)
            .AsQueryable();

        query = query.Where(c =>
            !c.IsDeleted
            || c.Replies.Any(r => !r.IsDeleted)
            || (c.QuotedCommentId != null &&
                _context.Comments.Any(qc => qc.CommentId == c.QuotedCommentId && !qc.IsDeleted))
        );
        

        query = (sortBy, ascending) switch
        {
            (CommentSortOption.Id, true) => query.OrderBy(c => c.CommentId),
            (CommentSortOption.Id, false) => query.OrderByDescending(c => c.CommentId),

            (CommentSortOption.CreatedAt, true) => query.OrderBy(c => c.CreatedAt),
            (CommentSortOption.CreatedAt, false) => query.OrderByDescending(c => c.CreatedAt),
            
            (CommentSortOption.Username, true) => query.OrderBy(c => c.User.Username),
            (CommentSortOption.Username, false) => query.OrderByDescending(c => c.User.Username),

            (CommentSortOption.MovieTitle, true) => query.OrderBy(c => c.Movie.Title),
            (CommentSortOption.MovieTitle, false) => query.OrderByDescending(c => c.Movie.Title),

            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        int totalCount = await query.CountAsync();

        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var commentDtos = new List<CommentDTO>();
        foreach (var comment in comments)
        {
            var dto = await BuildCommentDTOAsync(comment);
            commentDtos.Add(dto);
        }

        return (commentDtos, totalCount);
    }
    public async Task<CommentDTO?> GetCommentByIdAsync(int commentId)
    {
        var comment = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Movie)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        if (comment == null)
            return null;

        return await BuildCommentDTOAsync(comment);
    }

    public async Task<CommentDTO> AddCommentAsync(CreateCommentDTO dto, int userId)
    {
        // Optional: Validate movie exists

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
        
        var savedComment = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Movie)
            .FirstOrDefaultAsync(c => c.CommentId == comment.CommentId);

        if (savedComment is null)
        {
            throw new Exception("Could not reload saved comment");
        }

        return await BuildCommentDTOAsync(savedComment);
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _context.Comments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);

        if (comment == null) return false;

      
        if (comment.ParentCommentId == null)
        {
            bool hasActiveReplies = comment.Replies.Any(r => !r.IsDeleted);
            if (hasActiveReplies)
            {
                return false; 
            }
        }

        comment.IsDeleted = true;
        comment.Text = "Deleted";
        await _context.SaveChangesAsync();
        return true;
    }
    
     public async Task<bool> AdminDeleteCommentAsync(int commentId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        if (comment == null) return false;

        comment.IsDeleted = true;
        await _context.SaveChangesAsync();

        if (await IsCommentAndAllChildrenDeletedAsync(commentId))
        {
            await PermanentlyDeleteCommentAsync(commentId);
            await _context.SaveChangesAsync();
        }
        
        return true;
    }

    public async Task<List<CommentDTO>> GetCommentsByUserAsync(int userId)
    {
        var comments = await _context.Comments
            .Where(c => 
                c.UserId == userId && 
                c.ParentCommentId == null &&
                (
                    !c.IsDeleted
                    || c.Replies.Any(r => !r.IsDeleted)
                    || (c.QuotedCommentId != null &&
                        _context.Comments.Any(qc => qc.CommentId == c.QuotedCommentId && !qc.IsDeleted))
                )
            )
            .Include(c => c.User)
            .Include(c => c.Movie)
            .Include(c => c.Replies.Where(r => !r.IsDeleted))
            .ThenInclude(r => r.User)
            .ToListAsync();


       var commentDtos = new List<CommentDTO>();
       
       foreach (var comment in comments)
       {
           var dto = await BuildCommentDTOAsync(comment);
           commentDtos.Add(dto);
       }
       return commentDtos;
    }

    public async Task<List<CommentDTO>> GetCommentsByMovieAsync(int movieId)
    {
        var comments = await _context.Comments
            .Where(c => 
                c.MovieId == movieId && 
                c.ParentCommentId == null &&
                (
                    !c.IsDeleted
                    || c.Replies.Any(r => !r.IsDeleted)
                    || (c.QuotedCommentId != null &&
                        _context.Comments.Any(qc => qc.CommentId == c.QuotedCommentId && !qc.IsDeleted))
                )
            )
            .Include(c => c.User)
            .Include(c =>c.Movie)
            .Include(c => c.Replies)
            .Include(c => c.Replies.Where(r => !r.IsDeleted))
            .ThenInclude(r => r.User)
            .ToListAsync();

        var commentDtos = new List<CommentDTO>();

        foreach (var comment in comments)
        {
            var dto = await BuildCommentDTOAsync(comment);
            commentDtos.Add(dto);
        }

        return commentDtos;
    }


    public async Task<bool> LikeCommentAsync(int commentId, int userId)
        => await UpdateReactionAsync(commentId, userId, isLike: true);

    public async Task<bool> DislikeCommentAsync(int commentId, int userId)
        => await UpdateReactionAsync(commentId, userId, isLike: false);

    private async Task<bool> UpdateReactionAsync(int commentId, int userId, bool isLike)
    {
        var reaction = await _context.CommentReactions
            .FirstOrDefaultAsync(cr => cr.CommentId == commentId && cr.UserId == userId);

        if (reaction != null)
        {
            if (reaction.IsLike == isLike)
            {
                _context.CommentReactions.Remove(reaction);
            }
            else
            {
                reaction.IsLike = isLike;
                _context.CommentReactions.Update(reaction);
            }
        }
        else
        {
            var newReaction = new CommentReaction
            {
                CommentId = commentId,
                UserId = userId,
                IsLike = isLike
            };
            _context.CommentReactions.Add(newReaction);
        }

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
    // Load replies
    var replies = await _context.Comments
        .Where(r => r.ParentCommentId == comment.CommentId)
        .Include(r => r.User)
        .ToListAsync();

    // Load quoted comment
    Comment? quotedCommentEntity = null;
    if (comment.QuotedCommentId.HasValue)
    {
        quotedCommentEntity = await _context.Comments
            .Include(qc => qc.User)
            .FirstOrDefaultAsync(qc => qc.CommentId == comment.QuotedCommentId.Value);
    }

    // Build quoted DTO recursively (same as replies)
    CommentDTO? quotedCommentDTO = null;
    if (quotedCommentEntity != null)
    {
        // Recursively build DTO for quoted comment, to handle nested quotes/replies
        quotedCommentDTO = await BuildCommentDTOAsync(quotedCommentEntity);

        // If quoted comment is deleted, override text to "Deleted"
        if (quotedCommentEntity.IsDeleted)
        {
            quotedCommentDTO.Text = "Deleted";
        }
    }

    // Calculate reactions
    var reactionCounts = await _context.CommentReactions
        .Where(cr => cr.CommentId == comment.CommentId)
        .GroupBy(cr => cr.IsLike)
        .Select(g => new { IsLike = g.Key, Count = g.Count() })
        .ToListAsync();

    var likesCount = reactionCounts.FirstOrDefault(r => r.IsLike)?.Count ?? 0;
    var dislikesCount = reactionCounts.FirstOrDefault(r => !r.IsLike)?.Count ?? 0;

    // build replies sequentially to avoid concurrency exception
    var repliesDto = new List<CommentDTO>();
    foreach (var reply in replies)
    {
        var replyDto = await BuildCommentDTOAsync(reply);
        repliesDto.Add(replyDto);
    }

    // If current comment is deleted, show "Deleted" as text
    var commentText = comment.IsDeleted ? "Deleted" : comment.Text;

    return new CommentDTO
    {
        CommentId = comment.CommentId,
        Text = commentText,
        CreatedAt = comment.CreatedAt,
        UpdatedAt = comment.UpdatedAt,
        LikesCount = likesCount,
        DislikesCount = dislikesCount,
        MovieId = comment.MovieId,
        MovieTitle = comment.Movie.Title,
        UserId = comment.UserId,
        Username = comment.User?.Username ?? "Unknown",
        Author = comment.User != null ? $"{comment.User.FirstName} {comment.User.LastName}" : "N/A",
        ParentCommentId = comment.ParentCommentId,
        QuotedCommentId = comment.QuotedCommentId,
        QuotedText = quotedCommentEntity?.Text,
        QuotedComment = quotedCommentDTO,
        Replies = repliesDto
    };
}
private async Task<bool> IsCommentAndAllChildrenDeletedAsync(int commentId)
{
    var comment = await _context.Comments
        .Include(c => c.Replies)
        .FirstOrDefaultAsync(c => c.CommentId == commentId);

    if (comment == null) return true; // missing treated as deleted

    if (!comment.IsDeleted) return false; // active comment found

    // Check replies recursively
    foreach (var reply in comment.Replies)
    {
        bool childDeleted = await IsCommentAndAllChildrenDeletedAsync(reply.CommentId);
        if (!childDeleted) return false;
    }

    // Check quoted comment recursively
    if (comment.QuotedCommentId.HasValue)
    {
        bool quotedDeleted = await IsCommentAndAllChildrenDeletedAsync(comment.QuotedCommentId.Value);
        if (!quotedDeleted) return false;
    }

    return true; // all deleted
}
private async Task PermanentlyDeleteCommentAsync(int commentId)
{
    var comment = await _context.Comments
        .Include(c => c.Replies)
        .FirstOrDefaultAsync(c => c.CommentId == commentId);

    if (comment == null) return;

    // Delete replies recursively
    foreach (var reply in comment.Replies)
    {
        await PermanentlyDeleteCommentAsync(reply.CommentId);
    }

    // Delete the comment itself
    _context.Comments.Remove(comment);
}

}


