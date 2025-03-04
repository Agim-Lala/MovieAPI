namespace MovieAPI.Domain.Movies
{
    public class MovieDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public string DirectorName { get; set; }

        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}
