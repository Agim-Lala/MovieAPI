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
    public DateTime CreatedAt { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Approved;
    public UserRole Role { get; set; } = UserRole.Customer;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ReviewReaction> Reactions { get; set; } = new List<ReviewReaction>();  
    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
}

public enum UserRole
{
    Customer = 0,
    Admin = 1
}

public enum UserStatus
{
    Banned = 0,
    Approved = 1,
}