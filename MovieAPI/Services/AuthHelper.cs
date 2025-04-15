using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Domain.Users;

namespace MovieAPI.Services;

public class AuthHelper
{
    private readonly IConfiguration _config;

    public AuthHelper(IConfiguration config)
    {
        _config = config;
    }

    // Hash password
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Verify password
    public bool VerifyPassword(string hash, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    // Generate JWT Token
    public string GenerateToken(User user)
    {
        var claims = new[] 
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, ((int)user.Role).ToString()) 
        };

        // Update the key to be at least 128 bits (16 bytes)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])); 
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"], // Replace with your actual issuer
            audience: _config["Jwt:Audience"], // Replace with your actual audience
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}