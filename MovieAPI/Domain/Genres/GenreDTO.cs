using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Genres
{
    public class GenreDTO
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
    }
}
