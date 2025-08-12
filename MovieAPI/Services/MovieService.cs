using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Actors;
using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Directors;
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


        public async Task<(List<MovieDTO> Movies, int TotalCount)> GetSortedMoviesAsync(
            MovieSortOption sortBy,
            bool ascending = true,
            int page = 1,
            int pageSize = 10,
            string search = null 
        )
        {
            var moviesQuery = _context.Movies
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieQualities).ThenInclude(mq => mq.Quality)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLower();
                moviesQuery = moviesQuery.Where(m =>
                        m.Title.ToLower().Contains(lowered) ||
                        m.Description.ToLower().Contains(lowered)
                    
                );
            }


            // Apply sorting
            moviesQuery = (sortBy, ascending) switch
            {
                (MovieSortOption.Id, true) => moviesQuery.OrderBy(m => m.MovieId),
                (MovieSortOption.Id, false) => moviesQuery.OrderByDescending(m => m.MovieId),
                (MovieSortOption.Title, true) => moviesQuery.OrderBy(m => m.Title),
                (MovieSortOption.Title, false) => moviesQuery.OrderByDescending(m => m.Title),
                (MovieSortOption.Rating, true) => moviesQuery.OrderBy(m => m.AverageRating),
                (MovieSortOption.Rating, false) => moviesQuery.OrderByDescending(m => m.AverageRating),
                (MovieSortOption.Views, true) => moviesQuery.OrderBy(m => m.Views),
                (MovieSortOption.Views, false) => moviesQuery.OrderByDescending(m => m.Views),
                (MovieSortOption.CreatedAt, true) => moviesQuery.OrderBy(m => m.AddedAt),
                (MovieSortOption.CreatedAt, false) => moviesQuery.OrderByDescending(m => m.AddedAt),
                (MovieSortOption.Status, true) => moviesQuery.OrderBy(m => m.IsVisible),
                (MovieSortOption.Status, false) => moviesQuery.OrderByDescending(m => m.IsVisible),
                (MovieSortOption.Category, true) => moviesQuery.OrderBy(m =>
                    m.MovieCategories.FirstOrDefault().Category.Name),
                (MovieSortOption.Category, false) => moviesQuery.OrderByDescending(m =>
                    m.MovieCategories.FirstOrDefault().Category.Name),
                _ => moviesQuery.OrderByDescending(m => m.MovieId)
            };

            int totalCount = await moviesQuery.CountAsync();

            var movies = await moviesQuery
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
                .Where(m => m.IsVisible)
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
                .Where(m => m.IsVisible)
                .ToListAsync();

            return movies.Select(MapToDTO).ToList();
        }

        public async Task<GetMovieByIdDTO?> GetMovieByIdAsync(int movieId)
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

            return new GetMovieByIdDTO
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                ReleaseYear = movie.ReleaseYear,
                Description = movie.Description,
                Director = new DirectorDTO(movie.DirectorId, movie.Director.Name),

                Genres = movie.MovieGenres
                    .Select(mg => new GenreDTO(mg.Genre.GenreId, mg.Genre.Name))
                    .ToList(),

                Categories = movie.MovieCategories
                    .Select(mc => new CategoryDTO(mc.Category.CategoryId, mc.Category.Name))
                    .ToList(),

                Qualities = movie.MovieQualities
                    .Select(mq => new QualityDTO(mq.Quality.QualityId, mq.Quality.QualityName))
                    .ToList(),

                Actors = movie.MovieActors
                    .Select(ma => new ActorDTO(ma.Actor.ActorId, ma.Actor.ActorName))
                    .ToList(),
                AddedAt = movie.AddedAt,
                ImagePath = movie.ImagePath,
                VideoPath = movie.VideoPath,
                AverageRating = movie.AverageRating,
                RunningTime = movie.RunningTime,
                Age = movie.Age,
                Country = movie.Country,
                Link = movie.Link
            };
        }

        public async Task<MovieDTO> CreateMovieAsync(CreateMovieDTO dto)
        {
            
            string imagePath = "/Images/covers/default.jpg";
            if (!string.IsNullOrWhiteSpace(dto.CoverImage))
            {
                imagePath = await _fileUploadHelper.SaveBase64ToFileAsync(dto.CoverImage, "/Images/covers", ".jpg");
            }

            string videoPath = "Videos/default.mp4";
            if (!string.IsNullOrWhiteSpace(dto.VideoFile))
            {
                videoPath = await _fileUploadHelper.SaveBase64ToFileAsync(dto.VideoFile, "Videos", ".mp4");
            }


            var movie = new Movie
            {
                Title         = dto.Title,
                ReleaseYear   = dto.ReleaseYear,
                Description   = dto.Description,
                DirectorId    = dto.DirectorId,
                RunningTime   = dto.RunningTime,
                Link          = dto.Link,
                AddedAt       = dto.AddedAt,
                Country       = dto.Country,
                Age           = dto.Age,
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
                    Country      = m.Country,
                    Age          = m.Age, 
                    IsVisible    = m.IsVisible,
                    
                })
                .FirstAsync();

            return result;


        }

        
        public async Task<MovieDTO> UpdateMovieAsync(int id, UpdateMovieDTO dto)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres).ThenInclude(movieGenre => movieGenre.Genre)
                .Include(m => m.MovieCategories).ThenInclude(movieCategory => movieCategory.Category)
                .Include(m => m.MovieQualities).ThenInclude(movieQuality => movieQuality.Quality)
                .Include(m => m.MovieActors).ThenInclude(movieActor => movieActor.Actor)
                .Include(movie => movie.Director)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
                throw new Exception("Movie not found.");

            
            movie.Title = dto.Title;
            movie.ReleaseYear = dto.ReleaseYear;
            movie.Description = dto.Description;
            movie.DirectorId = dto.DirectorId;
            movie.RunningTime = dto.RunningTime;
            movie.Link = dto.Link;
            movie.Country = dto.Country;
            movie.Age = dto.Age;
            movie.Link = dto.Link;
            
            if (dto.CoverImage != null)
            {
                movie.ImagePath = await _fileUploadHelper.SaveBase64ToFileAsync(dto.CoverImage, "Images/covers", ".jpg");
            }

           
            if (dto.VideoFile != null)
            {
                movie.VideoPath = await _fileUploadHelper.SaveBase64ToFileAsync(dto.VideoFile, "Videos" , ".mp4" );
            }

            
            movie.MovieGenres = dto.GenreIds.Select(genreId => new MovieGenre { MovieId = id, GenreId = genreId }).ToList();
            movie.MovieCategories = dto.CategoryIds.Select(categoryId => new MovieCategory { MovieId = id, CategoryId = categoryId }).ToList();
            movie.MovieQualities = dto.QualityIds.Select(qualityId => new MovieQuality { MovieId = id, QualityId = qualityId }).ToList();
            movie.MovieActors = dto.ActorIds.Select(actorId => new MovieActor { MovieId = id, ActorId = actorId }).ToList();
            
            await _context.SaveChangesAsync();
            var categoryNames = await _context.MovieCategories
                .Where(mc => mc.MovieId == id)
                .Include(mc => mc.Category)
                .Select(mc => mc.Category.Name)
                .ToListAsync();

           
            return new MovieDTO
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                ReleaseYear = movie.ReleaseYear,
                Description = movie.Description,
                DirectorName = movie.Director.Name,
                Country = movie.Country,
                Age = movie.Age,
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
                Categories = categoryNames,
                Actors = movie.MovieActors.Select(ma => ma.Actor.ActorName).ToList(),
                Qualities = movie.MovieQualities.Select(mq => mq.Quality.QualityName).ToList(),
                IsVisible = movie.IsVisible,
                ImagePath = movie.ImagePath,
                Link = movie.Link
                
                
            };
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
                .Where(m => m.IsVisible)
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
        
        public async Task<List<MovieDTO>> GetFilteredMoviesAsync(int? genreId, int? startYear, int? endYear, int? qualityId, int page = 1, int pageSize= 18)
        {
            Console.WriteLine($"Filters Received -> GenreID: {genreId}, StartYear: {startYear}, EndYear: {endYear} ,QualityId: {qualityId}");

            var query = _context.Movies
                .Include(m => m.Director)
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Include(m => m.MovieQualities).ThenInclude(mq => mq.Quality)
                .Where(m => m.IsVisible)
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

            page = Math.Max(page, 1);

            var movies = await query
                .OrderBy(m => m.MovieId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Movies Found After Filtering: {movies.Count}");

            return movies.Select(MapToDTO).ToList();
        }
        
        public async Task<bool?> ToggleMovieVisibilityAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return null;

            movie.IsVisible = !movie.IsVisible;
            await _context.SaveChangesAsync();

            return movie.IsVisible;
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
            RunningTime = movie.RunningTime,
            Age = movie.Age,
            Country = movie.Country,
            Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
            Categories = movie.MovieCategories.Select(mc => mc.Category.Name).ToList(),
            Qualities = movie.MovieQualities?.Select(mq => mq.Quality?.QualityName).ToList() ?? new List<string>(),
            Actors = movie.MovieActors?.Select(ma => ma.Actor?.ActorName).ToList() ?? new List<string>(), 
            AddedAt = movie.AddedAt,
            ImagePath = movie.ImagePath,
            VideoPath = movie.VideoPath,
            Link = movie.Link,
            AverageRating = movie.AverageRating,
            IsVisible = movie.IsVisible,
            Views = movie.Views
        };
        

    }
}






































