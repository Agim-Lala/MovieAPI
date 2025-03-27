using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulateQualityAndMovieQualityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Qualities",
                columns: new[] { "QualityId", "QualityName" },
                values: new object[,]
                {
                    { 1, "1080p" },
                    { 2, "720p" },
                    { 3, "480p" }
                });
            
            migrationBuilder.InsertData(
    table: "MovieQualities",
    columns: new[] { "MovieId", "QualityId" },
    values: new object[,]
    {
        { 1, 1 }, // Jurassic Park: 1080p
        { 1, 2 }, // Jurassic Park: 720p
        { 1, 3 }, // Jurassic Park: 480p
        { 2, 1 }, // Inception: 1080p
        { 2, 2 }, // Inception: 720p
        { 2, 3 }, // Inception: 480p
        { 3, 1 }, // Pulp Fiction: 1080p
        { 3, 2 }, // Pulp Fiction: 720p
        { 3, 3 }, // Pulp Fiction: 480p
        { 4, 1 }, // Avatar: 1080p
        { 4, 2 }, // Avatar: 720p
        { 4, 3 }, // Avatar: 480p
        { 5, 1 }, // Spider-Man: 1080p
        { 5, 2 }, // Spider-Man: 720p
        { 5, 3 }, // Spider-Man: 480p
        { 6, 1 }, // Breaking Bad: 1080p
        { 6, 2 }, // Breaking Bad: 720p
        { 6, 3 }, // Breaking Bad: 480p
        { 7, 1 }, // The Witcher: 1080p
        { 7, 2 }, // The Witcher: 720p
        { 7, 3 }, // The Witcher: 480p
        { 8, 1 }, // Toy Story: 1080p
        { 8, 2 }, // Toy Story: 720p
        { 8, 3 }, // Toy Story: 480p
        { 9, 1 }, // The Godfather: 1080p
        { 9, 2 }, // The Godfather: 720p
        { 9, 3 }, // The Godfather: 480p
        { 10, 1 }, // Stranger Things: 1080p
        { 10, 2 }, // Stranger Things: 720p
        { 10, 3 }, // Stranger Things: 480p
        { 11, 1 }, // The Mandalorian: 1080p
        { 11, 2 }, // The Mandalorian: 720p
        { 11, 3 }, // The Mandalorian: 480p
        { 12, 1 }, // Interstellar: 1080p
        { 12, 2 }, // Interstellar: 720p
        { 12, 3 }, // Interstellar: 480p
        { 13, 1 }, // The Dark Knight: 1080p
        { 13, 2 }, // The Dark Knight: 720p
        { 13, 3 }, // The Dark Knight: 480p
        { 14, 1 }, // Friends: 1080p
        { 14, 2 }, // Friends: 720p
        { 14, 3 }, // Friends: 480p
        { 15, 1 }, // Shrek: 1080p
        { 15, 2 }, // Shrek: 720p
        { 15, 3 }, // Shrek: 480p
        { 16, 1 }, // Game of Thrones: 1080p
        { 16, 2 }, // Game of Thrones: 720p
        { 16, 3 }, // Game of Thrones: 480p
        { 17, 1 }, // The Lion King: 1080p
        { 17, 2 }, // The Lion King: 720p
        { 17, 3 }, // The Lion King: 480p
        { 18, 1 }, // The Matrix: 1080p
        { 18, 2 }, // The Matrix: 720p
        { 18, 3 }, // The Matrix: 480p
        { 21, 1 }, // Shrek 2: 1080p
        { 21, 2 }, // Shrek 2: 720p
        { 21, 3 }  // Shrek 2: 480p
    });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MovieQualities",
                keyColumns: new[] { "MovieId", "QualityId" },
                keyValues: new object[,]
                {
                    { 1, 1 }, { 1, 2 }, { 1, 3 },
                    { 2, 1 }, { 2, 2 }, { 2, 3 },
                    { 3, 1 }, { 3, 2 }, { 3, 3 },
                    { 4, 1 }, { 4, 2 }, { 4, 3 },
                    { 5, 1 }, { 5, 2 }, { 5, 3 },
                    { 6, 1 }, { 6, 2 }, { 6, 3 },
                    { 7, 1 }, { 7, 2 }, { 7, 3 },
                    { 8, 1 }, { 8, 2 }, { 8, 3 },
                    { 9, 1 }, { 9, 2 }, { 9, 3 },
                    { 10, 1 }, { 10, 2 }, { 10, 3 },
                    { 11, 1 }, { 11, 2 }, { 11, 3 },
                    { 12, 1 }, { 12, 2 }, { 12, 3 },
                    { 13, 1 }, { 13, 2 }, { 13, 3 },
                    { 14, 1 }, { 14, 2 }, { 14, 3 },
                    { 15, 1 }, { 15, 2 }, { 15, 3 },
                    { 16, 1 }, { 16, 2 }, { 16, 3 },
                    { 17, 1 }, { 17, 2 }, { 17, 3 },
                    { 18, 1 }, { 18, 2 }, { 18, 3 },
                    { 21, 1 }, { 21, 2 }, { 21, 3 }
                });

            // Reverse the Qualities insertions
            migrationBuilder.DeleteData(
                table: "Qualities",
                keyColumn: "QualityId",
                keyValues: new object[] { 1, 2, 3 });
        }

        }
    }

