using MovieAPI.Domain.Directors;



namespace MovieAPI.MovieEPI.Endpoints
{
    public static class DirectorEndpoints
    {
        public static void MapDirectorEndpoint(this WebApplication app)
        {
            
            List<Director> directors =
            [
                new () { DirectorId = 1, Name = "Christopher Nolan" },
                new () { DirectorId = 2, Name = "Steven Spielberg" }
            ];
            // Get all directors
            app.MapGet("/directors", () => directors.Select(d => new Director
            {
                DirectorId = d.DirectorId,
                Name = d.Name
                
            }));

            // Get director by ID
            app.MapGet("/directors/{id}", (int id) =>
            {
                var director = directors.FirstOrDefault(d => d.DirectorId == id);
                if (director == null) return Results.NotFound();

                  return Results.Ok(new DirectorDTO(director.DirectorId, director.Name));


            });

            // Create a director
            app.MapPost("/directors", (CreateDirectorDTO input) =>
            {
                var newId = directors.Any() ? directors.Max(d => d.DirectorId) + 1 : 1;

                var director = new Director
                {
                    DirectorId = newId,
                    Name = input.Name
                   
                };

                directors.Add(director);

                return Results.Created($"/directors/{director.DirectorId}", director);
            });


            app.MapPut("/director/{id}", (int id, CreateDirectorDTO input) =>
            {
                var director = directors.FirstOrDefault(d => d.DirectorId == id);
                if (director == null) return Results.NotFound();

                director.Name = input.Name;
               
                return Results.NoContent();
            });

            // Delete a director
            app.MapDelete("/directors/{id}", (int id) =>
            {
                var director = directors.FirstOrDefault(d => d.DirectorId == id);
                if (director == null) return Results.NotFound();

                directors.Remove(director);
                return Results.NoContent();
            });
        }

    }

    }

