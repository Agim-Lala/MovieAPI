namespace MovieAPI.Domain.Users;

public class UpdateUserProfileDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }   
    public string? Username { get; set; }
    public string? Email { get; set; }
    public int? SubscriptionPlanId { get; set; }
}