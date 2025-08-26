namespace MovieAPI.Domain.Users;

public class SubscriptionPlanDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int DurationInDays { get; set; } 
    public string Resolution { get; set; } = null!;
    public bool LifetimeAvailability { get; set; }
    public string DeviceAccess { get; set; } = null!;
    public string SupportLevel { get; set; } = null!;
}