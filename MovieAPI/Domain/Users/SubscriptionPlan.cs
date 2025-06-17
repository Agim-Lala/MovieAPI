namespace MovieAPI.Domain.Users;

public class SubscriptionPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string Duration { get; set; } = null!;
    public string Resolution { get; set; } = null!;
    public bool LifetimeAvailability { get; set; }
    public string DeviceAccess { get; set; } = null!;
    public string SupportLevel { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}