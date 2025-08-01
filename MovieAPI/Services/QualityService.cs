using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Qualities;

namespace MovieAPI.Services;

public class QualityService
{
    private readonly ApplicationDbContext _context;

    public QualityService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<QualityDTO>> GetAllQualitiesAsync()
    {
        return await _context.Qualities
            .Select(q => new QualityDTO (q.QualityId, q.QualityName ))
            .ToListAsync();
    }
}