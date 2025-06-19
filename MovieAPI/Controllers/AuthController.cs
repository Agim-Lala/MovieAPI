using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Domain.Users;
using MovieAPI.Enums;
using MovieAPI.Services;

namespace MovieAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("sorted")]
    public async Task<IActionResult> GetSortedUsersAsync([FromQuery] string sortBy, int page, int pageSize,
        [FromQuery] bool ascending = true)
    {
        if (!Enum.TryParse<UserSortOption>(sortBy, true, out var sortOption))
        {
            return BadRequest($"Invalid sort option: {sortBy}. Valid options are: {string.Join(", ", Enum.GetNames(typeof(UserSortOption)))}");
        }

        var (users, totalCount) = await _authService.GetSortedUsersAsync(sortOption, ascending, page, pageSize);
        return Ok(new {users,totalCount});
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Return validation errors
        }

        var token = await _authService.RegisterAsync(dto);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);
        return Ok(new { token });
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await _authService.GetMeAsync(User);
        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        return Ok(user);
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchUser(int id, [FromBody] UpdateUserProfileDto updatedUser)
    {
        if (updatedUser == null)
            return BadRequest("Invalid user data");

        
        var result = await _authService.UpdateUserAsync(id, updatedUser);
        return Ok(result);
    }
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
    {
        try
        {
            var result = await _authService.ChangePasswordAsync(id, dto);
            return result ? Ok("Password updated successfully.") : BadRequest("Password update failed.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserDto dto)
    {
        var updatedUser = await _authService.UpdateMeAsync(User, dto);
        return Ok(updatedUser);
    }
    
    [HttpPost("users/{id}/subscribe/{planId}")]
    public async Task<IActionResult> Subscribe(int id, int planId)
    {
        await _authService.AssignSubscriptionToUserAsync(id, planId);
        return Ok(new { message = "Subscription updated" });
    }
}