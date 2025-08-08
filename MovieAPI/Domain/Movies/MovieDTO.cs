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
        public List<string> Qualities { get; set; } = new List<string>();
        
        public List<string> Actors { get; set; } = new List<string>(); 
        public DateTime AddedAt { get; set; }
        
        public string ImagePath { get; set; }
        public string VideoPath {get; set;}
        
        public double AverageRating { get; set; } 
        public int RunningTime { get; set; }
        public int Age { get; set; }
        public string Country { get; set; }
        public bool IsVisible { get; set; }    
        
        public string Link { get; set; }
        public int Views { get; set; } 
    }
}
