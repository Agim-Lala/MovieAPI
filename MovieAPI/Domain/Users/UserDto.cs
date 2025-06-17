namespace MovieAPI.Domain.Users;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public int CommentCount { get; set; }
    public int ReviewCount { get; set; }
    public string  Status { get; set; }
    
    public string FullName { get; set; }
    
    public int SubscriptionPlanId { get; set; }
    
}