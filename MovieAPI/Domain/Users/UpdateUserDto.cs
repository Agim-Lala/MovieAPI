namespace MovieAPI.Domain.Users;

public class UpdateUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public string CurrentPassword { get; set; } 
    public string NewPassword { get; set; }
}