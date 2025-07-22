using MovieAPI.Domain.Genres;


namespace MovieAPI.MovieEPI.Endpoints
{
    public static class GenresEndpoint
    {
        public static void MapGenreEndpoint(this WebApplication app)
        {
            List<Genre> genres =
            [
                new () { GenreId = 1, Name = "Sci-Fi" },
                new () { GenreId = 2, Name = "Action" }
            ];

            // Get all genres
            app.MapGet("/genres", () => genres.Select(g => new Genre
            {
                GenreId = g.GenreId,
                Name = g.Name

            }));

            // Get director by ID
            app.MapGet("/Genres/{id}", (int id) =>
            {
                var genre = genres.FirstOrDefault(g => g.GenreId == id);
                if (genre == null) return Results.NotFound();

                return Results.Ok(new GenreDTO(genre.GenreId, genre.Name));
            });

            // Create a genre
            app.MapPost("/genre", (CreateGenreDTO input) =>
            {
                var newId = genres.Any() ? genres.Max(g => g.GenreId) + 1 : 1;

                var genre = new Genre
                {
                    GenreId = newId,
                    Name = input.Name

                };

                genres.Add(genre);

                return Results.Created($"/genres/{genre.GenreId}", genre);
            });


            app.MapPut("/genres/{id}", (int id, CreateGenreDTO input) =>
            {
                var genre = genres.FirstOrDefault(g => g.GenreId == id);
                if (genre == null) return Results.NotFound();

                genre.Name = input.Name;

                return Results.NoContent();
            });

            // Delete a genre
            app.MapDelete("/genres/{id}", (int id) =>
            {
                var genre = genres.FirstOrDefault(g => g.GenreId == id);
                if (genre == null) return Results.NotFound();

                genres.Remove(genre);
                return Results.NoContent();
            });
        }

    }
}

