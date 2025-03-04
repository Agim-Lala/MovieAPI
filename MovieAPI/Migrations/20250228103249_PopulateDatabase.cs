using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
 migrationBuilder.InsertData(
            table: "Categories",
            columns: new[] { "CategoryId", "Name" },
            values: new object[,]
            {
                { 1, "Action" },
                { 2, "Comedy" },
                { 3, "Drama" },
                { 4, "Horror" },
                { 5, "Sci-Fi" }
            });

        // Insert sample data into Directors table
        migrationBuilder.InsertData(
            table: "Directors",
            columns: new[] { "DirectorId", "Name" },
            values: new object[,]
            {
                { 1, "Steven Spielberg" },
                { 2, "Christopher Nolan" },
                { 3, "Quentin Tarantino" },
                { 4, "James Cameron" }
            });

        // Insert sample data into Genres table
        migrationBuilder.InsertData(
            table: "Genres",
            columns: new[] { "GenreId", "Name" },
            values: new object[,]
            {
                { 1, "Adventure" },
                { 2, "Action" },
                { 3, "Romance" },
                { 4, "Thriller" },
                { 5, "Fantasy" }
            });

        // Insert sample data into Movies table
        migrationBuilder.InsertData(
            table: "Movies",
            columns: new[] { "MovieId", "Title", "ReleaseYear", "Description", "DirectorId" },
            values: new object[,]
            {
                { 1, "Jurassic Park", 1993, "A theme park showcasing genetically resurrected dinosaurs turns into a nightmare when the security system fails.", 1 },
                { 2, "Inception", 2010, "A thief who enters the dreams of others to steal secrets from their subconscious is given the task of planting an idea in someone’s mind.", 2 },
                { 3, "Pulp Fiction", 1994, "The lives of two mob hitmen, a boxer, a gangster’s wife, and a pair of diner bandits intertwine in four tales of violence and redemption.", 3 },
                { 4, "Avatar", 2009, "A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.", 4 }
            });

        // Insert sample data into MovieCategories table
        migrationBuilder.InsertData(
            table: "MovieCategories",
            columns: new[] { "MovieId", "CategoryId" },
            values: new object[,]
            {
                { 1, 1 }, // Jurassic Park -> Action
                { 2, 1 }, // Inception -> Action
                { 3, 2 }, // Pulp Fiction -> Comedy
                { 4, 5 }  // Avatar -> Sci-Fi
            });

        // Insert sample data into MovieGenres table
        migrationBuilder.InsertData(
            table: "MovieGenres",
            columns: new[] { "MovieId", "GenreId" },
            values: new object[,]
            {
                { 1, 1 }, // Jurassic Park -> Adventure
                { 1, 2 }, // Jurassic Park -> Action
                { 2, 4 }, // Inception -> Thriller
                { 2, 5 }, // Inception -> Fantasy
                { 3, 2 }, // Pulp Fiction -> Action
                { 3, 3 }, // Pulp Fiction -> Romance
                { 4, 1 }  // Avatar -> Adventure
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
// Remove data in reverse order
        migrationBuilder.DeleteData(table: "MovieGenres", keyColumns: new[] { "MovieId", "GenreId" }, keyValues: new object[] { 1, 1 });
        migrationBuilder.DeleteData(table: "MovieGenres", keyColumns: new[] { "MovieId", "GenreId" }, keyValues: new object[] { 1, 2 });
        migrationBuilder.DeleteData(table: "MovieGenres", keyColumns: new[] { "MovieId", "GenreId" }, keyValues: new object[] { 2, 4 });
        migrationBuilder.DeleteData(table: "MovieGenres", keyColumns: new[] { "MovieId", "GenreId" }, keyValues: new object[] { 2, 5 });
        migrationBuilder.DeleteData(table: "MovieGenres", keyColumns: new[] { "MovieId", "GenreId" }, keyValues: new object[] { 3, 2 });
        migrationBuilder.DeleteData(table: "MovieGenres", keyColumns: new[] { "MovieId", "GenreId" }, keyValues: new object[] { 3, 3 });
        migrationBuilder.DeleteData(table: "MovieGenres", keyColumns: new[] { "MovieId", "GenreId" }, keyValues: new object[] { 4, 1 });

        migrationBuilder.DeleteData(table: "MovieCategories", keyColumns: new[] { "MovieId", "CategoryId" }, keyValues: new object[] { 1, 1 });
        migrationBuilder.DeleteData(table: "MovieCategories", keyColumns: new[] { "MovieId", "CategoryId" }, keyValues: new object[] { 2, 1 });
        migrationBuilder.DeleteData(table: "MovieCategories", keyColumns: new[] { "MovieId", "CategoryId" }, keyValues: new object[] { 3, 2 });
        migrationBuilder.DeleteData(table: "MovieCategories", keyColumns: new[] { "MovieId", "CategoryId" }, keyValues: new object[] { 4, 5 });

        migrationBuilder.DeleteData(table: "Movies", keyColumn: "MovieId", keyValue: 1);
        migrationBuilder.DeleteData(table: "Movies", keyColumn: "MovieId", keyValue: 2);
        migrationBuilder.DeleteData(table: "Movies", keyColumn: "MovieId", keyValue: 3);
        migrationBuilder.DeleteData(table: "Movies", keyColumn: "MovieId", keyValue: 4);

        migrationBuilder.DeleteData(table: "Genres", keyColumn: "GenreId", keyValue: 1);
        migrationBuilder.DeleteData(table: "Genres", keyColumn: "GenreId", keyValue: 2);
        migrationBuilder.DeleteData(table: "Genres", keyColumn: "GenreId", keyValue: 3);
        migrationBuilder.DeleteData(table: "Genres", keyColumn: "GenreId", keyValue: 4);
        migrationBuilder.DeleteData(table: "Genres", keyColumn: "GenreId", keyValue: 5);

        migrationBuilder.DeleteData(table: "Directors", keyColumn: "DirectorId", keyValue: 1);
        migrationBuilder.DeleteData(table: "Directors", keyColumn: "DirectorId", keyValue: 2);
        migrationBuilder.DeleteData(table: "Directors", keyColumn: "DirectorId", keyValue: 3);
        migrationBuilder.DeleteData(table: "Directors", keyColumn: "DirectorId", keyValue: 4);

        migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 1);
        migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 2);
        migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 3);
        migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 4);
        migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryId", keyValue: 5);
        }
    }
}
