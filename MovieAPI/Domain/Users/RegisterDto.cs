using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Domain.Users;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    
    public UserRole Role { get; set; } = UserRole.Customer; // remove this
}