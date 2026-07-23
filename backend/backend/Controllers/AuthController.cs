using backend.Models.DTOs.Requests;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { accessToken = result.AccessToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { accessToken = result.AccessToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies.TryGetValue("refresh_token", out var token) ? token : null;
            if (refreshToken is null)
                return Unauthorized();

            var result = await _authService.RefreshAsync(refreshToken);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { accessToken = result.AccessToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies.TryGetValue("refresh_token", out var token) ? token : null;
            if (refreshToken is null)
                return Unauthorized();

            await _authService.RevokeAsync(refreshToken);

            Response.Cookies.Delete("refresh_token");

            return NoContent();
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpirationDays"]!))
            };

            Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
        }
    }
}
