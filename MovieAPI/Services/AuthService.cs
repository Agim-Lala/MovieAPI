using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Users;

namespace MovieAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly AuthHelper _authHelper;

    public AuthService(ApplicationDbContext context, AuthHelper authHelper)
    {
        _context = context;
        _authHelper = authHelper;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        try
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists");
            
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username already exists");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = _authHelper.HashPassword(dto.Password),
                Role = UserRole.Customer
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _authHelper.GenerateToken(user);
        }
        catch (Exception ex)
        {
            
            throw new Exception("Error during registration", ex);
        }
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !_authHelper.VerifyPassword(user.PasswordHash, dto.Password)) 
            throw new Exception("Invalid credentials");

        return _authHelper.GenerateToken(user); // Generating token here
    }
    
    public async Task<UserDto> GetMeAsync(ClaimsPrincipal userClaims)
         {
             var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
             var user = await _context.Users.FindAsync(int.Parse(userId));
     
             if (user == null) throw new Exception("User not found");
     
             return new UserDto
             {
                 FirstName = user.FirstName,
                 LastName = user.LastName,
                 Username = user.Username,
                 Email = user.Email,
                 Role = user.Role.ToString() 

             };
         }
    
    public async Task<UserDto> UpdateMeAsync(ClaimsPrincipal userClaims, UpdateUserDto dto)
    {
        var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _context.Users.FindAsync(int.Parse(userId));
        if (user == null) throw new Exception("User not found");

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != user.Id);
            if (emailExists) throw new Exception("Email is already in use");
            user.Email = dto.Email;
        }
        
        if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
        {
            var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != user.Id);
            if (usernameExists) throw new Exception("Username is already in use");
            user.Username = dto.Username;
        }

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        
        
        if (!string.IsNullOrWhiteSpace(dto.CurrentPassword) && !string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (!_authHelper.VerifyPassword(user.PasswordHash, dto.CurrentPassword))
                throw new Exception("Current password is incorrect");

            user.PasswordHash = _authHelper.HashPassword(dto.NewPassword);
        }

        await _context.SaveChangesAsync();

        return new UserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Email = user.Email
        };
    }
}