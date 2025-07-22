using MovieAPI.Domain.Categories;


namespace MovieAPI.MovieEPI.Endpoints
{
    public static class CategoryEndpoint
    {
        public static void MapCategoryEndpoint(this WebApplication app)
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Blockbuster" },
                new Category { CategoryId = 2, Name = "Classic" }
            };

            // Get all categories
            app.MapGet("/categories", () => categories.Select(c => new Category
            {
                CategoryId = c.CategoryId,
                Name = c.Name

            }));

            // Get categories by ID
            app.MapGet("/categories/{id}", (int id) =>
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null) return Results.NotFound();

                return Results.Ok(new CategoryDTO( category.CategoryId,category.Name));
            });

            // Create a category
            app.MapPost("/categories", (CreateCategoryDTO input) =>
            {
                var newId = categories.Any() ? categories.Max(c => c.CategoryId) + 1 : 1;

                var category = new Category
                {
                    CategoryId = newId,
                    Name = input.Name

                };

                categories.Add(category);

                return Results.Created($"/category/{category.CategoryId}", category);
            });


            app.MapPut("/category/{id}", (int id, CreateCategoryDTO input) =>
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null) return Results.NotFound();

                category.Name = input.Name;

                return Results.NoContent();
            });

            // Delete a category
            app.MapDelete("/category/{id}", (int id) =>
            {
                var category = categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null) return Results.NotFound();

                categories.Remove(category);
                return Results.NoContent();
            });
        }
    }
}
