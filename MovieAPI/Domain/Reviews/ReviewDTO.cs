namespace MovieAPI.Domain.Reviews;

public class ReviewDTO
{
    public int ReviewId { get; set; }
    public string Text { get; set; } = "";
    public double Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Username { get; set; } = "";
    
    public string MovieTitle { get; set; } = ""; 
    
    public int MovieId {get; set;} 
    
    public int UserId { get; set; }


}