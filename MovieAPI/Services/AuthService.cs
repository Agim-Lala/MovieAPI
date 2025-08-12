using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Users;
using MovieAPI.Enums;

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

    public async Task<(List<UserDto> Users, int TotalCount)> GetSortedUsersAsync(UserSortOption sortBy,
        bool ascending = true, int page = 1, int pageSize = 10)
    {
        var query = _context.Users.AsQueryable();

        query = (sortBy, ascending) switch
        {
            (UserSortOption.Id, true) => query.OrderBy(u => u.Id),
            (UserSortOption.Id, false) => query.OrderByDescending(u => u.Id),
            (UserSortOption.FirstName, true) => query.OrderBy(u => u.FirstName),
            (UserSortOption.FirstName, false) => query.OrderByDescending(u => u.FirstName),
            (UserSortOption.LastName, true) => query.OrderBy(u => u.LastName),
            (UserSortOption.LastName, false) => query.OrderByDescending(u => u.LastName),
            (UserSortOption.Email, true) => query.OrderBy(u => u.Email),
            (UserSortOption.Email, false) => query.OrderByDescending(u => u.Email),
            (UserSortOption.Username, true) => query.OrderBy(u => u.Username),
            (UserSortOption.Username, false) => query.OrderByDescending(u => u.Username),
            (UserSortOption.CreatedAt, true) => query.OrderBy(u => u.CreatedAt),
            (UserSortOption.CreatedAt, false) => query.OrderByDescending(u => u.CreatedAt),
            (UserSortOption.CommentCount, true) => query.OrderBy(u => _context.Comments.Count(c => c.UserId == u.Id)),
            (UserSortOption.CommentCount, false) => query.OrderByDescending(u => _context.Comments.Count(c => c.UserId == u.Id)),
            (UserSortOption.ReviewCount, true) => query.OrderBy(u => _context.Reviews.Count(r => r.UserId == u.Id)),
            (UserSortOption.ReviewCount, false) => query.OrderByDescending(u => _context.Reviews.Count(r => r.UserId == u.Id)),

            _ => query.OrderBy(u => u.Id)
        };

        int totalCount = await query.CountAsync();
        
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users.Select(MapToDTO).ToList(), totalCount);
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
                Role = UserRole.Customer,
                SubscriptionPlanId = 1, 
                SubscriptionStartDate = DateTime.UtcNow,
                SubscriptionEndDate = null
                
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
     
             return MapToDTO(user);

         }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new Exception("User not found");
        
        return MapToDTO(user);
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

        return MapToDTO(user);
    }
    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserProfileDto updatedDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new Exception("User not found");

        if (!string.IsNullOrWhiteSpace(updatedDto.Username) &&
            updatedDto.Username != user.Username &&
            await _context.Users.AnyAsync(u => u.Username == updatedDto.Username && u.Id != id))
        {
            throw new Exception("Username is already taken by another user.");
        }

        if (!string.IsNullOrWhiteSpace(updatedDto.Email) &&
            updatedDto.Email != user.Email &&
            await _context.Users.AnyAsync(u => u.Email == updatedDto.Email && u.Id != id))
        {
            throw new Exception("Email is already in use by another user.");
        }

        if (!string.IsNullOrWhiteSpace(updatedDto.FirstName))
            user.FirstName = updatedDto.FirstName;

        if (!string.IsNullOrWhiteSpace(updatedDto.LastName))
            user.LastName = updatedDto.LastName;

        if (!string.IsNullOrWhiteSpace(updatedDto.Username))
            user.Username = updatedDto.Username;

        if (!string.IsNullOrWhiteSpace(updatedDto.Email))
            user.Email = updatedDto.Email;
       
        if (updatedDto.SubscriptionPlanId.HasValue)
        {
            var planExists = await _context.SubscriptionPlans
                .AnyAsync(sp => sp.Id == updatedDto.SubscriptionPlanId.Value);

            if (!planExists)
                throw new Exception("Subscription plan not found.");

            user.SubscriptionPlanId = updatedDto.SubscriptionPlanId.Value;
            user.SubscriptionStartDate = DateTime.UtcNow;
            user.SubscriptionEndDate = null; 
        }
        await _context.SaveChangesAsync();

        return MapToDTO(user);
    }
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new Exception("User not found.");

        if (!_authHelper.VerifyPassword(user.PasswordHash, dto.OldPassword))
            throw new Exception("Old password is incorrect.");

        if (dto.NewPassword != dto.ConfirmNewPassword)
            throw new Exception("New password and confirmation do not match.");

        user.PasswordHash = _authHelper.HashPassword(dto.NewPassword);

        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<UserStatus?> ToggleUserStatusAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null; // or throw an exception if you prefer

        if (user.Status == UserStatus.Approved)
        {
            user.Status = UserStatus.Banned;
        }
        else if (user.Status == UserStatus.Banned)
        {
            user.Status = UserStatus.Approved;
        }

        await _context.SaveChangesAsync();
        return user.Status;
    }
    
    public async Task AssignSubscriptionToUserAsync(int userId, int planId)
    {
        var user = await _context.Users.FindAsync(userId);
        var plan = await _context.SubscriptionPlans.FindAsync(planId);
        if (user == null || plan == null)
            throw new Exception("User or plan not found");

        user.SubscriptionPlanId = planId;
        user.SubscriptionStartDate = DateTime.UtcNow;
        user.SubscriptionEndDate = plan.LifetimeAvailability
            ? null
            : DateTime.UtcNow.AddDays(ParseDurationInDays(plan.Duration));

        await _context.SaveChangesAsync();
    }

    private UserDto MapToDTO(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Username = user.Username,
            CommentCount = _context.Comments.Count(c => c.UserId == user.Id),
            ReviewCount = _context.Reviews.Count(r => r.UserId == user.Id),
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            Role = user.Role.ToString(),
            SubscriptionPlanId = user.SubscriptionPlanId
        };
    }
    
    private int ParseDurationInDays(string duration) {
        if (duration.Contains("day")) return int.Parse(duration.Split(' ')[0]);
        if (duration.Contains("month")) return int.Parse(duration.Split(' ')[0]) * 30;
        throw new Exception("Invalid duration format");
    }
    
}