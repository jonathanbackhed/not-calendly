using backend.Data;
using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbc;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext dbc, IConfiguration config, ILogger<AuthService> logger)
        {
            _dbc = dbc;
            _config = config;
            _logger = logger;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _dbc.Users.SingleOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant());
            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for email {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            _logger.LogInformation("User logged in with email {Email}", request.Email);
            return await IssueTokensAsync(user);
        }

        public async Task<AuthResponse> RefreshAsync(string refreshToken)
        {
            var stored = await _dbc.RefreshTokens
                .Include(i => i.User)
                .SingleOrDefaultAsync(s => s.Token == refreshToken);

            if (stored is null)
            {
                _logger.LogWarning("Refresh attempt with unknown token");
                throw new UnauthorizedAccessException("Invalid token.");
            }

            if (stored.RevokedAt is not null || stored.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh attempt with revoked or expired token for user {User}", stored.UserId);
                throw new UnauthorizedAccessException("Token expired or revoked.");
            }

            stored.RevokedAt = DateTime.UtcNow;
            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Token refreshed for user {UserId}", stored.UserId);
            return await IssueTokensAsync(stored.User!);
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existing = await _dbc.Users
                .Where(u => u.Email == request.Email.ToLowerInvariant()
                    || u.Username.ToLower() == request.Username.ToLower()
                    || u.Slug.ToLower() == request.Slug.ToLower())
                .Select(u => new { u.Email, u.Username, u.Slug })
                .FirstOrDefaultAsync();

            if (existing is not null)
            {
                if (existing.Email == request.Email.ToLowerInvariant())
                {
                    _logger.LogWarning("Registration attempt with existing email {Email}", request.Email);
                    throw new InvalidOperationException("Email already in use.");
                }
                if (existing.Username.ToLower() == request.Username.ToLower())
                {
                    _logger.LogWarning("Registration attempt with existing username {Username}", request.Username);
                    throw new InvalidOperationException("Username already in use.");
                }
                if (existing.Slug.ToLower() == request.Slug.ToLower())
                {
                    _logger.LogWarning("Registration attempt with existing slug {Slug}", request.Slug);
                    throw new InvalidOperationException("Slug already in use.");
                }
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
                Username = request.Username,
                Slug = request.Slug,
                CreatedAt = DateTime.UtcNow,
            };

            _dbc.Users.Add(user);
            await _dbc.SaveChangesAsync();

            _logger.LogInformation("User registered with email {Email}", user.Email);
            return await IssueTokensAsync(user);
        }

        public async Task RevokeAsync(string refreshToken)
        {
            var stored = await _dbc.RefreshTokens.SingleOrDefaultAsync(s => s.Token == refreshToken);
            if (stored is null)
            {
                _logger.LogWarning("User tried to logout with invalid refresh token {RefreshToken}", refreshToken);
                return;
            }

            stored.RevokedAt = DateTime.UtcNow;
            await _dbc.SaveChangesAsync();
            _logger.LogInformation("User {UserId} logged out", stored.UserId);
        }

        private async Task<AuthResponse> IssueTokensAsync(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user);

            return new AuthResponse(accessToken, refreshToken);
        }

        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var expiry = int.Parse(_config["Jwt:AccessTokenExpirationMinutes"]!);

            var token = new JwtSecurityToken(
                claims: [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())],
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var expiryDays = int.Parse(_config["Jwt:RefreshTokenExpirationDays"]!);

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbc.RefreshTokens.Add(refreshToken);
            await _dbc.SaveChangesAsync();

            return refreshToken.Token;
        }
    }
}
