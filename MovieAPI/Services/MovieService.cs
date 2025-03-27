using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Movies;
using MovieAPI.Domain.Qualities;

namespace MovieAPI.Services
{
    public class MovieService
    {
        private readonly ApplicationDbContext _context;

        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        
        public async Task<List<MovieDTO>> GetNewMoviesAsync(int page, int pageSize)
        {
            var movies = await _context.Movies
                .OrderByDescending(m => m.AddedAt) 
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
        }
        public async Task<List<MovieDTO>> GetAllMoviesAsync()
        {
            var movies = await _context.Movies

                .OrderByDescending(m => m.ReleaseYear)
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .Include(m => m.MovieQualities)
                .ThenInclude(mq => mq.Quality)
                .Include(m => m.Director)
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
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
                    ImagePath = m.ImagePath,
                    AddedAt = m.AddedAt,
                    Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                    Categories = m.MovieCategories.Select(mc => mc.Category.Name).ToList(),
                    Qualities = m.MovieQualities.Select(mq => mq.Quality.QualityName).ToList(),
                    
                    
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
            
            var qualities = await _context.Qualities.Where(q => movieDTO.QualityIds.Contains(q.QualityId)).ToListAsync();
            if (qualities.Count != movieDTO.QualityIds.Count) throw new Exception("One or more qualities are invalid");
            

            var movieGenres = genres.Select(g => new MovieGenre { GenreId = g.GenreId }).ToList();
            var movieCategories = categories.Select(c => new MovieCategory { CategoryId = c.CategoryId }).ToList();
            var movieQualities = qualities.Select(q => new MovieQuality { QualityId = q.QualityId }).ToList(); 

            
            

            var movie = new Movie
            {
                Title = movieDTO.Title,
                ReleaseYear = movieDTO.ReleaseYear,
                Description = movieDTO.Description,
                DirectorId = movieDTO.DirectorId,
                MovieGenres = movieGenres,
                MovieCategories = movieCategories,
                MovieQualities = movieQualities,
                AddedAt = DateTime.UtcNow,
                ImagePath = movieDTO.ImagePath 

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
                Categories = categories.Select(c => c.Name).ToList(),
                Qualities = qualities.Select(q => q.QualityName).ToList(),
                AddedAt =movie.AddedAt,
                ImagePath = movie.ImagePath 

                
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
                .OrderByDescending(m => m.ReleaseYear)
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
        
        public async Task<List<MovieDTO>> GetFilteredMoviesAsync(int? genreId, int? startYear, int? endYear, int? qualityId)
        {
            Console.WriteLine($"Filters Received -> GenreID: {genreId}, StartYear: {startYear}, EndYear: {endYear} ,QualityId: {qualityId}");

            var query = _context.Movies
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Include(m => m.MovieQualities).ThenInclude(mq => mq.Quality) 
                .AsQueryable();

            if (genreId.HasValue)
            {
                query = query.Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId));
            }

            if (startYear.HasValue && endYear.HasValue)
            {
                query = query.Where(m => m.ReleaseYear >= startYear && m.ReleaseYear <= endYear);
            }
            else if (startYear.HasValue) 
            {
                query = query.Where(m => m.ReleaseYear >= startYear);
            }
            else if (endYear.HasValue)
            {
                query = query.Where(m => m.ReleaseYear <= endYear);
            }
            if (qualityId.HasValue)
            {
                query = query.Where(m => m.MovieQualities.Any(mq => mq.QualityId == qualityId));
            }

            var movies = await query.ToListAsync();

            Console.WriteLine($"Movies Found After Filtering: {movies.Count}");

            return movies.Select(MapToDTO).ToList();
        }
        
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid image upload.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadsFolder); 

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{uniqueFileName}"; 
        }
        
        private MovieDTO MapToDTO(Movie movie) => new MovieDTO
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            ReleaseYear = movie.ReleaseYear,
            Description = movie.Description,
            DirectorName = movie.Director.Name,
            Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
            Categories = movie.MovieCategories.Select(mc => mc.Category.Name).ToList(),
            Qualities = movie.MovieQualities?.Select(mq => mq.Quality?.QualityName).ToList() ?? new List<string>(),
            AddedAt = movie.AddedAt,
            ImagePath = movie.ImagePath,
        };
        

    }
}






































