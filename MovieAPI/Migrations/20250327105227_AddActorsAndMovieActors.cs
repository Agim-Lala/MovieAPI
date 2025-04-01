using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddActorsAndMovieActors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActorName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            // Insert initial data into Actors table
            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "ActorName" },
                values: new object[,]
                {
                    { "Sam Neill" }, // Jurassic Park
                    { "Laura Dern" }, // Jurassic Park
                    { "Jeff Goldblum" }, // Jurassic Park
                    { "Leonardo DiCaprio" }, // Inception
                    { "Joseph Gordon-Levitt" }, // Inception
                    { "Elliot Page" }, // Inception
                    { "Samuel L. Jackson" }, // Pulp Fiction
                    { "John Travolta" }, // Pulp Fiction
                    { "Uma Thurman" }, // Pulp Fiction
                    { "Sam Worthington" }, // Avatar
                    { "Zoe Saldana" }, // Avatar
                    { "Sigourney Weaver" }, // Avatar
                    { "Tobey Maguire" }, // Spider-Man
                    { "Kirsten Dunst" }, // Spider-Man
                    { "Willem Dafoe" }, // Spider-Man
                    { "Bryan Cranston" }, // Breaking Bad
                    { "Aaron Paul" }, // Breaking Bad
                    { "Anna Gunn" }, // Breaking Bad
                    { "Henry Cavill" }, // The Witcher
                    { "Anya Chalotra" }, // The Witcher
                    { "Freya Allan" }, // The Witcher
                    { "Tom Hanks" }, // Toy Story
                    { "Tim Allen" }, // Toy Story
                    { "Don Vito Corleone" }, //The Godfather (Fictional, but to represent the role)
                    { "Marlon Brando"}, //The Godfather
                    { "Al Pacino"}, //The Godfather
                    { "Winona Ryder" }, // Stranger Things
                    { "David Harbour" }, // Stranger Things
                    { "Millie Bobby Brown" }, // Stranger Things
                    { "Pedro Pascal" }, // The Mandalorian
                    { "Giancarlo Esposito" }, // The Mandalorian
                    { "Katee Sackhoff" }, // The Mandalorian
                    { "Matthew McConaughey" }, // Interstellar
                    { "Anne Hathaway" }, // Interstellar
                    { "Jessica Chastain" }, // Interstellar
                    { "Christian Bale" }, // The Dark Knight
                    { "Heath Ledger" }, // The Dark Knight
                    { "Aaron Eckhart" }, // The Dark Knight
                    { "Jennifer Aniston" }, // Friends
                    { "Courteney Cox" }, // Friends
                    { "Lisa Kudrow" }, // Friends
                    { "Mike Myers"}, //Shrek
                    { "Eddie Murphy"}, //Shrek
                    { "Cameron Diaz"}, //Shrek
                    { "Kit Harington" }, // Game of Thrones
                    { "Emilia Clarke" }, // Game of Thrones
                    { "Peter Dinklage" }, // Game of Thrones
                    { "James Earl Jones"}, //The Lion King
                    { "Matthew Broderick"}, //The Lion King
                    { "Jeremy Irons"}, //The Lion King
                    { "Keanu Reeves" }, // The Matrix
                    { "Laurence Fishburne" }, // The Matrix
                    { "Carrie-Anne Moss" }, // The Matrix
                    { "Mike Myers"}, //Shrek 2
                    { "Eddie Murphy"}, //Shrek 2
                    { "Cameron Diaz"}, //Shrek 2
                });
        
        migrationBuilder.CreateTable(
                name: "MovieActors",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    ActorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieActors", x => new { x.MovieId, x.ActorId });
                    table.ForeignKey(
                        name: "FK_MovieActors_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieActors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Insert data into MovieActors table
            migrationBuilder.InsertData(
                table: "MovieActors",
                columns: new[] { "MovieId", "ActorId" },
                values: new object[,]
                {
                    { 1, 1 }, // Jurassic Park - Sam Neill
                    { 1, 2 }, // Jurassic Park - Laura Dern
                    { 1, 3 }, // Jurassic Park - Jeff Goldblum
                    { 2, 4 }, // Inception - Leonardo DiCaprio
                    { 2, 5 }, // Inception - Joseph Gordon-Levitt
                    { 2, 6 }, // Inception - Elliot Page
                    { 3, 7 }, // Pulp Fiction - Samuel L. Jackson
                    { 3, 8 }, // Pulp Fiction - John Travolta
                    { 3, 9 }, // Pulp Fiction - Uma Thurman
                    { 4, 10 }, // Avatar - Sam Worthington
                    { 4, 11 }, // Avatar - Zoe Saldana
                    { 4, 12 }, // Avatar - Sigourney Weaver
                    { 5, 13 }, // Spider-Man - Tobey Maguire
                    { 5, 14 }, // Spider-Man - Kirsten Dunst
                    { 5, 15 }, // Spider-Man - Willem Dafoe
                    { 6, 16 }, // Breaking Bad - Bryan Cranston
                    { 6, 17 }, // Breaking Bad - Aaron Paul
                    { 6, 18 }, // Breaking Bad - Anna Gunn
                    { 7, 19 }, // The Witcher - Henry Cavill
                    { 7, 20 }, // The Witcher - Anya Chalotra
                    { 7, 21 }, // The Witcher - Freya Allan
                    { 8, 22 }, // Toy Story - Tom Hanks
                    { 8, 23 }, // Toy Story - Tim Allen
                    { 9, 24}, // The Godfather - Marlon Brando
                    { 9, 25}, //The Godfather - Al Pacino
                    { 10, 26 }, // Stranger Things - Winona Ryder
                    { 10, 27 }, // Stranger Things - David Harbour
                    { 10, 28 }, // Stranger Things - Millie Bobby Brown
                    { 11, 29 }, // The Mandalorian - Pedro Pascal
                    { 11, 30 }, // The Mandalorian - Giancarlo Esposito
                    { 11, 31 }, // The Mandalorian - Katee Sackhoff
                    { 12, 32 }, // Interstellar - Matthew McConaughey
                    { 12, 33 }, // Interstellar - Anne Hathaway
                    { 12, 34 }, // Interstellar - Jessica Chastain
                    { 13, 35 }, // The Dark Knight - Christian Bale
                    { 13, 36 }, // The Dark Knight - Heath Ledger
                    { 13, 37 }, // The Dark Knight - Aaron Eckhart
                    { 14, 38 }, // Friends - Jennifer Aniston
                    { 14, 39 }, // Friends - Courteney Cox
                    { 14, 40 }, // Friends - Lisa Kudrow
                    { 15, 41 }, //Shrek - Mike Myers
                    { 15, 42 }, //Shrek - Eddie Murphy
                    { 15, 43 }, //Shrek - Cameron Diaz
                    { 16, 44 }, // Game of Thrones - Kit Harington
                    { 16, 45 }, // Game of Thrones - Emilia Clarke
                    { 16, 46 }, // Game of Thrones - Peter Dinklage
                    { 17, 47 }, // The Lion King - James Earl Jones
                    { 17, 48 }, // The Lion King - Matthew Broderick
                    { 17, 49 }, // The Lion King - Jeremy Irons
                    { 18, 50 }, // The Matrix - Keanu Reeves
                    { 18, 51 }, // The Matrix - Laurence Fishburne
                    { 18, 52 }, // The Matrix - Carrie-Anne Moss
                    { 21, 41 }, //Shrek 2 - Mike Myers
                    { 21, 42 }, //Shrek 2 - Eddie Murphy
                    { 21, 43 }, //Shrek 2 - Cameron Diaz

                });
            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_ActorId",
                table: "MovieActors",
                column: "ActorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieActors");

            migrationBuilder.DropTable(
                name: "Actors");
        }
    }
}
