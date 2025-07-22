using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Actors;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Movies;
using MovieAPI.Domain.Qualities;
using MovieAPI.Domain.Users;
using MovieAPI.Enums;

namespace MovieAPI.Services
{
    public class MovieService
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadHelper _fileUploadHelper;

        public MovieService(ApplicationDbContext context, FileUploadHelper fileUploadHelper)
        {
            _context = context;
            _fileUploadHelper = fileUploadHelper;

        }
        
        
        public async Task<(List<MovieDTO> Movies, int TotalCount)> GetSortedMoviesAsync(MovieSortOption sortBy, bool ascending = true, int page =1, int pageSize=10)
        {
            var query = _context.Movies
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieQualities).ThenInclude(mq => mq.Quality)
                .AsQueryable();

            // Apply sorting
            query = (sortBy, ascending) switch
            {
                (MovieSortOption.Id, true) => query.OrderBy(m => m.MovieId),
                (MovieSortOption.Id, false) => query.OrderByDescending(m => m.MovieId),
                (MovieSortOption.Title, true) => query.OrderBy(m => m.Title),
                (MovieSortOption.Title, false) => query.OrderByDescending(m => m.Title),
                (MovieSortOption.Rating, true) => query.OrderBy(m => m.AverageRating),
                (MovieSortOption.Rating, false) => query.OrderByDescending(m => m.AverageRating),
                (MovieSortOption.Views, true) => query.OrderBy(m => m.Views),
                (MovieSortOption.Views, false) => query.OrderByDescending(m => m.Views),
                (MovieSortOption.CreatedAt, true) => query.OrderBy(m => m.AddedAt),
                (MovieSortOption.CreatedAt, false) => query.OrderByDescending(m => m.AddedAt),
                (MovieSortOption.Status, true) => query.OrderBy(m => m.IsVisible),
                (MovieSortOption.Status, false) => query.OrderByDescending(m => m.IsVisible),
                (MovieSortOption.Category, true) => query.OrderBy(m => m.MovieCategories.FirstOrDefault().Category.Name),
                (MovieSortOption.Category, false) => query.OrderByDescending(m => m.MovieCategories.FirstOrDefault().Category.Name),
                _ => query.OrderByDescending(m => m.MovieId)
            };

            int totalCount = await query.CountAsync();
            
            var movies= await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)     
                .ToListAsync();

            return (movies.Select(MapToDTO).ToList(), totalCount);

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
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor) 
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
        }

        public async Task<MovieDTO?> GetMovieByIdAsync(int movieId)
        {
            var movie = await _context.Movies
                .Where(m => m.MovieId == movieId)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Include(m => m.MovieQualities).ThenInclude(mq => mq.Quality)
                .Include(m => m.Director)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor) 
                .FirstOrDefaultAsync();

            if (movie == null) return null;

            return MapToDTO(movie);
        }

        public async Task<MovieDTO> CreateMovieAsync(CreateMovieDTO dto)
        {
            
            var imagePath = await _fileUploadHelper.UploadAsync(dto.CoverImage, "Images/covers") ?? "cover.jpg";
            var videoPath = await _fileUploadHelper.UploadAsync(dto.VideoFile,  "Videos") ?? "movie.mp4";


            var movie = new Movie
            {
                Title         = dto.Title,
                ReleaseYear   = dto.ReleaseYear,
                Description   = dto.Description,
                DirectorId    = dto.DirectorId,
                RunningTime   = dto.RunningTime,
                Link          = dto.Link,
                AddedAt       = dto.AddedAt,
                ImagePath     = imagePath,
                VideoPath     = videoPath,
                MovieGenres   = dto.GenreIds.Select(id => new MovieGenre { GenreId = id }).ToList(),
                MovieCategories = dto.CategoryIds.Select(id => new MovieCategory { CategoryId = id }).ToList(),
                MovieQualities = dto.QualityIds.Select(id => new MovieQuality { QualityId = id }).ToList(),
                MovieActors   = dto.ActorIds.Select(id => new MovieActor { ActorId = id }).ToList()
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            var result = await _context.Movies
                .Where(m => m.MovieId == movie.MovieId)
                .Select(m => new MovieDTO
                {
                    MovieId      = m.MovieId,
                    Title        = m.Title,
                    ReleaseYear  = m.ReleaseYear,
                    Description  = m.Description,
                    DirectorName = m.Director.Name,
                    Genres       = m.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                    Categories   = m.MovieCategories.Select(mc => mc.Category.Name).ToList(),
                    Qualities    = m.MovieQualities.Select(mq => mq.Quality.QualityName).ToList(),
                    Actors       = m.MovieActors.Select(ma => ma.Actor.ActorName).ToList(),
                    AddedAt      = m.AddedAt,
                    ImagePath    = m.ImagePath,
                    VideoPath    = m.VideoPath,
                    AverageRating= m.AverageRating,
                    IsVisible    = m.IsVisible,
                    Views        = m.Views
                })
                .FirstAsync();

            return result;


        }

        
        public async Task<bool> UpdateMovieAsync(int id, CreateMovieDTO updatedMovie)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieCategories)
                .Include(m => m.MovieActors)
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
            
            _context.MovieActors.RemoveRange(movie.MovieActors);
            movie.MovieActors = updatedMovie.ActorIds
                .Select(aid => new MovieActor { ActorId = aid, MovieId = movie.MovieId })
                .ToList();

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteMovieAsync(int movieId)
        {
           
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)  
                .Include(m => m.MovieCategories)  
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.MovieId == movieId);

           
            if (movie == null) return false;

            
            _context.MovieGenres.RemoveRange(movie.MovieGenres);
            _context.MovieCategories.RemoveRange(movie.MovieCategories);
            _context.MovieActors.RemoveRange(movie.MovieActors);
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
        
        //VIEWS 

        public async Task RecordMovieViewAsync(int movieId, int userId)
        {
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null) throw new Exception("Movie not found");

            var watch = new UserMovieWatch
            {
                MovieId = movieId,
                UserId = userId,
                WatchedAt = DateTime.UtcNow,
            };
            await _context.UserMovieWatches.AddAsync(watch);

            movie.Views += 1;

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalViewsAsync(int movieId)
        {
            var movie = await _context.Movies.FindAsync(movieId);
            return movie?.Views ??0;
        }
        
        public async Task<int> GetAllMoviesMonthlyUniqueViewsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfNextMonth = startOfMonth.AddMonths(1);

            var uniqueViews = await _context.UserMovieWatches
                .Where(umw => umw.WatchedAt >= startOfMonth && umw.WatchedAt < startOfNextMonth)
                .GroupBy(umw => umw.MovieId)
                .Select(g => g.Select(x => x.UserId).Distinct().Count())
                .SumAsync();

            return uniqueViews;
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
            Actors = movie.MovieActors?.Select(ma => ma.Actor?.ActorName).ToList() ?? new List<string>(), 
            AddedAt = movie.AddedAt,
            ImagePath = movie.ImagePath,
            VideoPath = movie.VideoPath,
            AverageRating = movie.AverageRating,
            IsVisible = movie.IsVisible,
            Views = movie.Views
        };
        

    }
}






































