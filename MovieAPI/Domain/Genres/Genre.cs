using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Genres
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    }
}
