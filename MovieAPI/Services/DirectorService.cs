using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Directors;

namespace MovieAPI.Services;

public class DirectorService
{
     private readonly ApplicationDbContext _context;

        public DirectorService(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<DirectorDTO>> GetAllDirectorsAsync()
        {
            return await _context.Directors
                .Select(d => new DirectorDTO(d.DirectorId, d.Name))
                .ToListAsync();
        }

        
        public async Task<DirectorDTO?> GetDirectorByIdAsync(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null) return null;

            return new DirectorDTO(director.DirectorId, director.Name);
        }
        

        
        public async Task<DirectorDTO> CreateDirectorAsync(CreateDirectorDTO createDirectorDto)
        {
            var director = new Director { Name = createDirectorDto.Name };
            _context.Directors.Add(director);
            await _context.SaveChangesAsync();

         
            return new DirectorDTO(director.DirectorId, createDirectorDto.Name);
        }

  
        public async Task<DirectorDTO?> UpdateDirectorAsync(int id, DirectorDTO directorDTO)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null) return null;

            
            director.Name = directorDTO.Name;
            await _context.SaveChangesAsync();

           
            return new DirectorDTO(director.DirectorId, director.Name);
        }

      
        public async Task<bool> DeleteDirectorAsync(int id)
        {
            var director = await _context.Directors.FindAsync(id);
            if (director == null) return false;

            _context.Directors.Remove(director);
            await _context.SaveChangesAsync();

            return true;
        }
    
}