
using ManagingTaskApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskManagementApi.Data;
using TaskManagementApi.Filters;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace ManagingTaskApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly LoginLogger _loginLogger;

    public AuthController(TokenService tokenService, LoginLogger loginLogger)
    {
        _tokenService = tokenService;
        _loginLogger = loginLogger;
    }

    [HttpPost("login")]
    [RequireApiKey]
    [EnableRateLimiting("loginPolicy")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = UserStore.Users.FirstOrDefault(u =>
            u.Username == request.Username && u.Password == request.Password);

        if (string.IsNullOrWhiteSpace(user.Username))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var jwtToken = _tokenService.CreateJwtToken(user.Username, user.Role);
        var refreshToken = _tokenService.CreateRefreshToken();

        RefreshTokenStore.Tokens[refreshToken] = (
            user.Username,
            user.Role,
            DateTime.UtcNow.AddDays(1)
        );

        _loginLogger.LogLogin(user.Username, user.Role, true);

        return Ok(new LoginResponse
        {
            JwtToken = jwtToken,
            RefreshToken = refreshToken,
            Username = user.Username,
            Role = user.Role
        });
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("loginPolicy")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        if (!RefreshTokenStore.Tokens.TryGetValue(request.RefreshToken, out var tokenInfo))
        {
            return Unauthorized(new { message = "Invalid refresh token." });
        }

        if (tokenInfo.ExpiresAt <= DateTime.UtcNow)
        {
            RefreshTokenStore.Tokens.Remove(request.RefreshToken);
            return Unauthorized(new { message = "Refresh token expired." });
        }

        var newJwt = _tokenService.CreateJwtToken(tokenInfo.Username, tokenInfo.Role);
        var newRefresh = _tokenService.CreateRefreshToken();

        RefreshTokenStore.Tokens.Remove(request.RefreshToken);
        RefreshTokenStore.Tokens[newRefresh] = (
            tokenInfo.Username,
            tokenInfo.Role,
            DateTime.UtcNow.AddDays(1)
        );

        return Ok(new
        {
            jwtToken = newJwt,
            refreshToken = newRefresh
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout([FromBody] RefreshRequest request)
    {
        if (RefreshTokenStore.Tokens.ContainsKey(request.RefreshToken))
        {
            RefreshTokenStore.Tokens.Remove(request.RefreshToken);
        }

        return Ok(new { message = "Logged out successfully. Refresh token invalidated." });
    }
}