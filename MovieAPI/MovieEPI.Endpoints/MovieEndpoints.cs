using MovieAPI.Domain.Categories;
using MovieAPI.Domain.Directors;
using MovieAPI.Domain.Genres;
using MovieAPI.Domain.Movies;

namespace MovieAPI.MovieEPI.Endpoints
{
    public static class MinimalApiMovies
    {
        public static void MapMovieEndpoints(this WebApplication app)
        {
            var directors = new List<Director>
            {
                new Director { DirectorId = 1, Name = "Christopher Nolan" },
                new Director { DirectorId = 2, Name = "Steven Spielberg" }
            };

            var genres = new List<Genre>
            {
                new Genre { GenreId = 1, Name = "Sci-Fi" },
                new Genre { GenreId = 2, Name = "Action" }
            };

            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Movies"},
                new Category { CategoryId = 2, Name = "TV Series"},
                new Category { CategoryId = 3, Name = "Cartoon"}
            };

            var movies = new List<Movie>
            {
                new Movie
                {
                    MovieId = 1,
                    Title = "Inception",
                    ReleaseYear = 2010,
                    Description = "A mind-bending thriller.",
                    DirectorId = 1,
                    MovieGenres = new List<MovieGenre> { new MovieGenre { GenreId = 1 } },
                    MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = 1 } }
                }
            };

            // Get all movies
            app.MapGet("/movies", () => movies.Select(m => new MovieDTO
            {
                MovieId = m.MovieId,
                Title = m.Title,
                ReleaseYear = m.ReleaseYear,
                Description = m.Description,
                DirectorName = directors.FirstOrDefault(d => d.DirectorId == m.DirectorId)?.Name ?? "Unknown",
                Genres = m.MovieGenres.Select(mg => genres.FirstOrDefault(g => g.GenreId == mg.GenreId)?.Name ?? "Unknown").ToList(),
                Categories = m.MovieCategories.Select(mc => categories.FirstOrDefault(c => c.CategoryId == mc.CategoryId)?.Name ?? "Unknown").ToList()
            }));

            // Get movie by ID
            app.MapGet("/movies/{id}", (int id) =>
            {
                var movie = movies.FirstOrDefault(m => m.MovieId == id);
                if (movie == null) return Results.NotFound();

                return Results.Ok(new MovieDTO
                {
                    MovieId = movie.MovieId,
                    Title = movie.Title,
                    ReleaseYear = movie.ReleaseYear,
                    Description = movie.Description,
                    DirectorName = directors.FirstOrDefault(d => d.DirectorId == movie.DirectorId)?.Name ?? "Unknown",
                    Genres = movie.MovieGenres.Select(mg => genres.FirstOrDefault(g => g.GenreId == mg.GenreId)?.Name ?? "Unknown").ToList(),
                    Categories = movie.MovieCategories.Select(mc => categories.FirstOrDefault(c => c.CategoryId == mc.CategoryId)?.Name ?? "Unknown").ToList()
                });
            });

            // Create a movie
            app.MapPost("/movies", (CreateMovieDTO input) =>
            {
                var newId = movies.Any() ? movies.Max(m => m.MovieId) + 1 : 1;

                var movie = new Movie
                {
                    MovieId = newId,
                    Title = input.Title,
                    ReleaseYear = input.ReleaseYear,
                    Description = input.Description,
                    DirectorId = input.DirectorId,
                    MovieGenres = input.GenreIds.Select(gid => new MovieGenre { GenreId = gid }).ToList(),
                    MovieCategories = input.CategoryIds.Select(cid => new MovieCategory { CategoryId = cid }).ToList()
                };

                movies.Add(movie);

                return Results.Created($"/movies/{movie.MovieId}", movie);
            });

            // Update a movie
            app.MapPut("/movies/{id}", (int id, CreateMovieDTO input) =>
            {
                var movie = movies.FirstOrDefault(m => m.MovieId == id);
                if (movie == null) return Results.NotFound();

                movie.Title = input.Title;
                movie.ReleaseYear = input.ReleaseYear;
                movie.Description = input.Description;
                movie.DirectorId = input.DirectorId;
                movie.MovieGenres = input.GenreIds.Select(gid => new MovieGenre { GenreId = gid }).ToList();
                movie.MovieCategories = input.CategoryIds.Select(cid => new MovieCategory { CategoryId = cid }).ToList();

                return Results.NoContent();
            });

            // Delete a movie
            app.MapDelete("/movies/{id}", (int id) =>
            {
                var movie = movies.FirstOrDefault(m => m.MovieId == id);
                if (movie == null) return Results.NotFound();

                movies.Remove(movie);
                return Results.NoContent();
            });
        }

    }
}
