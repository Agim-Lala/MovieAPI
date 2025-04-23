using MovieAPI.Domain.Comments;

namespace MovieAPI.Domain.Users;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }  
    public UserRole Role { get; set; } = UserRole.Customer;
    
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    public ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
    
    public List<Comment> LikedComments { get; set; } = new();
    
    public List<Comment> DislikedComments { get; set; } = new();

    
}

public enum UserRole
{
    Customer = 0,
    Admin = 1
}