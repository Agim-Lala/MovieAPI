using System.Security.Claims;
using MovieAPI.Domain.Users;
using MovieAPI.Enums;

namespace MovieAPI.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<UserDto> GetMeAsync(ClaimsPrincipal user);
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> UpdateMeAsync(ClaimsPrincipal user, UpdateUserDto dto);

    Task<(List<UserDto> Users, int TotalCount)> GetSortedUsersAsync(UserSortOption sortBy,
        bool ascending = true, int page = 1, int pageSize = 10);

    Task<UserDto> UpdateUserAsync(int id, UpdateUserProfileDto userDto);
    Task AssignSubscriptionToUserAsync(int userId, int planId);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
}