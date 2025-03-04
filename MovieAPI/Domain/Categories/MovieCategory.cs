using MovieAPI.Domain.Movies;

namespace MovieAPI.Domain.Categories
{
    public class MovieCategory
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
