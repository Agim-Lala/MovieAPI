﻿namespace MovieAPI.Domain.Users;

public class UserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    
    public string Role { get; set; }
    
}