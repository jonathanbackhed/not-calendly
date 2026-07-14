using backend.Data;
using backend.Exceptions;
using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Services
{
    public class AvailabilityOverrideService : IAvailabilityOverrideService
    {
        private readonly AppDbContext _dbc;
        private readonly ILogger<AvailabilityOverrideService> _logger;

        public AvailabilityOverrideService(AppDbContext dbc, ILogger<AvailabilityOverrideService> logger)
        {
            _dbc = dbc;
            _logger = logger;
        }

        public async Task<AvailabilityOverrideResponse> CreateAsync(Guid userId, AvailabilityOverrideRequest request)
        {
            if (request.OverrideDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ValidationException("Override date cannot be in the past.");

            if (!request.IsBlocked && (request.StartTime is null || request.EndTime is null))
                throw new ValidationException("StartTime and EndTime are required when the day is not blocked.");

            if (request.StartTime.HasValue && request.EndTime.HasValue && request.StartTime >= request.EndTime)
                throw new ValidationException("StartTime must be before EndTime.");

            var exists = await _dbc.AvailabilityOverrides.AnyAsync(r => r.UserId == userId && r.OverrideDate == request.OverrideDate);
            if (exists)
                throw new ConflictException("Override for this day already exists.");

            var overrideEntity = new AvailabilityOverride()
            {
                UserId = userId,
                OverrideDate = request.OverrideDate,
                IsBlocked = request.IsBlocked,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                CreatedAt = DateTime.UtcNow
            };

            _dbc.AvailabilityOverrides.Add(overrideEntity);
            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Availability override created for user {UserId} on {OverrideDate}", userId, overrideEntity.CreatedAt);

            return new AvailabilityOverrideResponse
            {
                Id = overrideEntity.Id,
                OverrideDate = overrideEntity.OverrideDate,
                IsBlocked = overrideEntity.IsBlocked,
                StartTime = overrideEntity.StartTime,
                EndTime = overrideEntity.EndTime,
                CreatedAt = overrideEntity.CreatedAt
            };
        }

        public async Task DeleteAsync(Guid userId, Guid overrideId)
        {
            var deleted = await _dbc.AvailabilityOverrides
                .Where(r => r.Id == overrideId && r.UserId == userId)
                .ExecuteDeleteAsync();

            if (deleted == 0)
                throw new NotFoundException("Availability override not found.");

            _logger.LogInformation("Availability override {OverrideId} deleted by user {Userid}", overrideId, userId);
        }

        public async Task<IEnumerable<AvailabilityOverrideResponse>> GetAsync(Guid userId)
        {
            var overrides = await _dbc.AvailabilityOverrides
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Select(r => new AvailabilityOverrideResponse
                {
                    Id = r.Id,
                    OverrideDate = r.OverrideDate,
                    IsBlocked = r.IsBlocked,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return overrides;
        }

        public async Task<AvailabilityOverrideResponse> UpdateAsync(Guid userId, Guid overrideId, AvailabilityOverrideRequest request)
        {
            if (request.OverrideDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ValidationException("Override date cannot be in the past.");

            if (!request.IsBlocked && (request.StartTime is null || request.EndTime is null))
                throw new ValidationException("StartTime and EndTime are required when the day is not blocked.");

            if (request.StartTime.HasValue && request.EndTime.HasValue && request.StartTime >= request.EndTime)
                throw new ValidationException("StartTime must be before EndTime.");

            var overrideEntity = await _dbc.AvailabilityOverrides.FirstOrDefaultAsync(r => r.Id == overrideId && r.UserId == userId);
            if (overrideEntity is null)
                throw new NotFoundException("Availability override not found.");

            overrideEntity.OverrideDate = request.OverrideDate;
            overrideEntity.IsBlocked = request.IsBlocked;
            overrideEntity.StartTime = request.StartTime;
            overrideEntity.EndTime = request.EndTime;
            overrideEntity.UpdatedAt = DateTime.UtcNow;

            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Availability override {OverrideId} updated by user {UserId}", overrideId, userId);

            return new AvailabilityOverrideResponse
            {
                Id = overrideEntity.Id,
                OverrideDate = overrideEntity.OverrideDate,
                IsBlocked = overrideEntity.IsBlocked,
                StartTime = overrideEntity.StartTime,
                EndTime = overrideEntity.EndTime,
                CreatedAt = overrideEntity.CreatedAt
            };
        }
    }
}
