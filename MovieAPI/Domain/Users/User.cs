using MovieAPI.Domain.Comments;
using MovieAPI.Domain.Reviews;

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
    
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    public ICollection<ReviewReaction> Reactions { get; set; } = new List<ReviewReaction>();  


    
}

public enum UserRole
{
    Customer = 0,
    Admin = 1
}