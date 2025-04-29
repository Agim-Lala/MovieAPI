using System.Security.Claims;
using MovieAPI.Domain.Users;

namespace MovieAPI.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<UserDto> GetMeAsync(ClaimsPrincipal user);
    Task<UserDto> UpdateMeAsync(ClaimsPrincipal user, UpdateUserDto dto);
}