using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Actors;


namespace MovieAPI.Services;

public class ActorService
{
    private readonly ApplicationDbContext _context;

    public ActorService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<ActorDTO>> GetAllActorsAsync()
    {
        return await _context.Actors
            .Select(a => new ActorDTO (a.ActorId, a.ActorName ))
            .ToListAsync();
    }
}