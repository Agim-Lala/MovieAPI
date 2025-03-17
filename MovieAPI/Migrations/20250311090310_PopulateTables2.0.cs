using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class PopulateTables20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
    table: "Directors",
    columns: new[] { "DirectorId", "Name" },
    values: new object[,]
    {
        { 5, "Lana Wachowski" },
        { 6, "Lilly Wachowski" },
        { 7, "Vince Gilligan" },
        { 8, "Lauren Schmidt Hissrich" },
        { 9, "John Lasseter" },
        { 10, "Francis Ford Coppola" },
        { 11, "The Duffer Brothers" },
        { 12, "Jon Favreau" },
        { 13, "David Crane & Marta Kauffman" },
        { 14, "Andrew Adamson & Vicky Jenson" },
        { 15, "David Benioff & D. B. Weiss" },
        { 16, "Roger Allers & Rob Minkoff" }
    });

// Insert new Movies
migrationBuilder.InsertData(
    table: "Movies",
    columns: new[] { "MovieId", "Title", "ReleaseYear", "Description", "DirectorId" },
    values: new object[,]
    {
        { 18, "The Matrix", 1999, "A computer hacker learns about the true nature of reality and his role in the war against its controllers.", 5 },
        { 6, "Breaking Bad", 2008, "A high school chemistry teacher turned methamphetamine producer navigates the dangers of the drug trade.", 7 },
        { 7, "The Witcher", 2019, "A mutated monster hunter struggles to find his place in a world where people often prove more wicked than beasts.", 8 },
        { 8, "Toy Story", 1995, "A cowboy doll is profoundly threatened and jealous when a new spaceman figure supplants him as top toy in a boy's room.", 9 },
        { 9, "The Godfather", 1972, "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.", 10 },
        { 10, "Stranger Things", 2016, "A group of young friends uncover a series of supernatural mysteries in their small town.", 11 },
        { 11, "The Mandalorian", 2019, "A lone bounty hunter makes his way through the galaxy’s outer reaches, far from the authority of the New Republic.", 12 },
        { 12, "Interstellar", 2014, "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.", 2 },
        { 13, "The Dark Knight", 2008, "Batman faces off against the Joker, a criminal mastermind who seeks to create chaos in Gotham.", 2 },
        { 14, "Friends", 1994, "Follows the personal and professional lives of six friends living in Manhattan.", 13 },
        { 15, "Shrek", 2001, "An ogre's peaceful swamp is invaded, leading him on a quest to rescue a princess.", 14 },
        { 16, "Game of Thrones", 2011, "Noble families wage war to control the Iron Throne and rule the Seven Kingdoms of Westeros.", 15 },
        { 17, "The Lion King", 1994, "A lion cub prince flees his kingdom only to learn the true meaning of responsibility and bravery.", 16 }
    });

// Insert new MovieCategories
migrationBuilder.InsertData(
    table: "MovieCategories",
    columns: new[] { "MovieId", "CategoryId" },
    values: new object[,]
    {
        { 18, 1 }, // The Matrix -> Movies
        { 6, 2 }, // Breaking Bad -> TV Series
        { 7, 2 }, // The Witcher -> TV Series
        { 8, 3 }, // Toy Story -> Cartoons
        { 9, 1 },  // The Godfather -> Movies
        { 10, 2 }, // Stranger Things -> TV Series
        { 11, 2 }, // The Mandalorian -> TV Series
        { 12, 1 }, // Interstellar -> Movies
        { 13, 1 }, // The Dark Knight -> Movies
        { 14, 2 }, // Friends -> TV Series
        { 15, 3 }, // Shrek -> Cartoons
        { 16, 2 }, // Game of Thrones -> TV Series
        { 17, 3 }  // The Lion King -> Cartoons
    });

// Insert new MovieGenres
migrationBuilder.InsertData(
    table: "MovieGenres",
    columns: new[] { "MovieId", "GenreId" },
    values: new object[,]
    {
        { 18, 7 }, // The Matrix -> Sci-Fi
        { 18, 2 }, // The Matrix -> Action
        { 6, 4 }, // Breaking Bad -> Thriller
        { 7, 5 }, // The Witcher -> Fantasy
        { 7, 2 }, // The Witcher -> Action
        { 8, 1 }, // Toy Story -> Adventure
        { 8, 2 }, // Toy Story -> Action
        { 9, 3 }, // The Godfather -> Romance (for drama-like categorization)
        { 10, 7 }, // Stranger Things -> Sci-Fi
        { 10, 6 }, // Stranger Things -> Mystery
        { 11, 7 }, // The Mandalorian -> Sci-Fi
        { 11, 2 }, // The Mandalorian -> Action
        { 12, 7 }, // Interstellar -> Sci-Fi
        { 12, 1 }, // Interstellar -> Adventure
        { 13, 2 }, // The Dark Knight -> Action
        { 13, 4 }, // The Dark Knight -> Thriller
        { 14, 2 }, // Friends -> Comedy
        { 14, 3 }, // Friends -> Romance
        { 15, 2 }, // Shrek -> Comedy
        { 15, 1 }, // Shrek -> Adventure
        { 16, 5 }, // Game of Thrones -> Fantasy
        { 16, 1 }, // Game of Thrones -> Adventure
        { 17, 1 }, // The Lion King -> Adventure
        { 17, 3 }  // The Lion King -> Romance (Drama-like)
    });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
