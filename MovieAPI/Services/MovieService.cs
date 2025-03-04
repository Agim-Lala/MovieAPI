using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Directors;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Movies;

namespace MovieAPI.Services
{
    public class MovieService
    {
        private readonly ApplicationDbContext _context;

        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<MovieDTO>> GetAllMoviesAsync()
        {
            var movies = await _context.Movies


                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .Include(m => m.Director)
                .ToListAsync();

            var movieDTOs = movies.Select(m => new MovieDTO
            {
                MovieId = m.MovieId,
                Title = m.Title,
                ReleaseYear = m.ReleaseYear,
                Description = m.Description,
                DirectorName = m.Director.Name,
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                Categories = m.MovieCategories.Select(mc => mc.Category.Name).ToList()
            }).ToList();

            return movieDTOs;
        }

        public async Task<MovieDTO?> GetMovieByIdAsync(int movieId)
        {
            return await _context.Movies
                .Where(m => m.MovieId == movieId)
                .Select(m => new MovieDTO
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    ReleaseYear = m.ReleaseYear,
                    Description = m.Description,
                    DirectorName = m.Director.Name,
                    Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                    Categories = m.MovieCategories.Select(mc => mc.Category.Name).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MovieDTO> AddMovieAsync(CreateMovieDTO movieDTO)
        {
            var director = await _context.Directors.FindAsync(movieDTO.DirectorId);
            if (director == null) throw new Exception("Director not found");

            var genres = await _context.Genres.Where(g => movieDTO.GenreIds.Contains(g.GenreId)).ToListAsync();
            if (genres.Count != movieDTO.GenreIds.Count) throw new Exception("One or more genres are invalid");

            var categories = await _context.Categories.Where(c => movieDTO.CategoryIds.Contains(c.CategoryId)).ToListAsync();
            if (categories.Count != movieDTO.CategoryIds.Count) throw new Exception("One or more categories are invalid");

            var movieGenres = genres.Select(g => new MovieGenre { GenreId = g.GenreId }).ToList();
            var movieCategories = categories.Select(c => new MovieCategory { CategoryId = c.CategoryId }).ToList();

            var movie = new Movie
            {
                Title = movieDTO.Title,
                ReleaseYear = movieDTO.ReleaseYear,
                Description = movieDTO.Description,
                DirectorId = movieDTO.DirectorId,
                MovieGenres = movieGenres,
                MovieCategories = movieCategories,
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            
            return new MovieDTO
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                ReleaseYear = movie.ReleaseYear,
                Description = movie.Description,
                DirectorName = director.Name,
                Genres = genres.Select(g => g.Name).ToList(),
                Categories = categories.Select(c => c.Name).ToList()
            };
        }
        
        public async Task<bool> UpdateMovieAsync(int id, CreateMovieDTO updatedMovie)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieCategories)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null) return false;

            
            movie.Title = updatedMovie.Title;
            movie.ReleaseYear = updatedMovie.ReleaseYear;
            movie.Description = updatedMovie.Description;
            movie.DirectorId = updatedMovie.DirectorId;

            
            _context.MovieGenres.RemoveRange(movie.MovieGenres);
            movie.MovieGenres = updatedMovie.GenreIds
                .Select(gid => new MovieGenre { GenreId = gid, MovieId = movie.MovieId })
                .ToList();

            
            _context.MovieCategories.RemoveRange(movie.MovieCategories);
            movie.MovieCategories = updatedMovie.CategoryIds
                .Select(cid => new MovieCategory { CategoryId = cid, MovieId = movie.MovieId })
                .ToList();

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteMovieAsync(int movieId)
        {
           
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)  
                .Include(m => m.MovieCategories)  
                .FirstOrDefaultAsync(m => m.MovieId == movieId);

           
            if (movie == null) return false;

            
            _context.Movies.Remove(movie);

          
            await _context.SaveChangesAsync();

            return true;  
        }
        
        public async Task<List<MovieDTO>> GetMoviesByCategoryIdAsync(int categoryId)
        {
            var movies = await _context.Movies
                .Where(m => m.MovieCategories.Any(mc => mc.CategoryId == categoryId))
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
        }

        public async Task<List<MovieDTO>> GetMoviesByDirectorIdAsync(int directorId)
        {
            var movies = await _context.Movies
                .Where(m =>m.DirectorId == directorId)
                .Include(m => m.Director)
                .Include(m => m.MovieGenres). ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
        }

        public async Task<List<MovieDTO>> GetMoviesByGenreIdAsync(int genreId)
        {
            var movies = await _context.Movies
                .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
        }

        public async Task<List<MovieDTO>> GetMoviesByReleaseYearAsync(int startYear, int endYear)
        {
            var movies = await _context.Movies
                .Where(m => m.ReleaseYear >= startYear && m.ReleaseYear <= endYear)
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
        }
        
        private MovieDTO MapToDTO(Movie movie) => new MovieDTO
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Description = movie.Description,
            DirectorName = movie.Director.Name,
            Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
            Categories = movie.MovieCategories.Select(mc => mc.Category.Name).ToList()
        };
        

    }
}






































