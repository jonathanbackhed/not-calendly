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

        public AuthService(AppDbContext dbc, IConfiguration config)
        {
            _dbc = dbc;
            _config = config;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _dbc.Users.SingleOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant())
                ?? throw new UnauthorizedAccessException("Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResponse> RefreshAsync(string refreshToken)
        {
            var stored = await _dbc.RefreshTokens
                .Include(i => i.User)
                .SingleOrDefaultAsync(s => s.Token == refreshToken)
                    ?? throw new UnauthorizedAccessException("Invalid token.");

            if (stored.RevokedAt is not null || stored.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Token expired or revoked.");

            stored.RevokedAt = DateTime.UtcNow;
            await _dbc.SaveChangesAsync();

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
                    throw new InvalidOperationException("Email already in use.");
                if (existing.Username.ToLower() == request.Username.ToLower())
                    throw new InvalidOperationException("Username already in use.");
                if (existing.Slug.ToLower() == request.Slug.ToLower())
                    throw new InvalidOperationException("Slug already in use.");
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

            return await IssueTokensAsync(user);
        }

        public async Task RevokeAsync(string refreshToken)
        {
            var stored = await _dbc.RefreshTokens.SingleOrDefaultAsync(s => s.Token == refreshToken);
            if (stored is null) return;

            stored.RevokedAt = DateTime.UtcNow;
            await _dbc.SaveChangesAsync();
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
