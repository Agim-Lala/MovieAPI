using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Genres;

namespace MovieAPI.Services;

public class GenresService
{
    private readonly ApplicationDbContext _context;

    public GenresService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<GenreDTO>> GetAllGenresAsync()
    {
        return await _context.Genres
            .Select(g => new GenreDTO {GenreId = g.GenreId , Name = g.Name}) 
            .ToListAsync();
    }

    public async Task<GenreDTO?> GetGenreByIdAsync(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null) return null;
        
        return new GenreDTO {GenreId = genre.GenreId, Name = genre.Name};
    }

    public async Task<GenreDTO> CreateGenreAsync(CreateGenreDTO genreDto)
    {
        var genre = new Genre {Name = genreDto.Name};
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        
        return new GenreDTO {GenreId = genre.GenreId, Name = genreDto.Name};
    }

    public async Task<GenreDTO?> UpdateGenreAsync(int id, GenreDTO genreDto)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null) return null;
        
        genre.Name = genreDto.Name;
        await _context.SaveChangesAsync();
        
        return new GenreDTO {GenreId = genre.GenreId, Name = genreDto.Name};
    }

    public async Task<bool> DeleteGenreAsync(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null) return false;
        
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();

        return true;
    }
    
    
    
    
    
    
    
}