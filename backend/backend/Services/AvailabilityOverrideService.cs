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

        public AvailabilityOverrideService(AppDbContext dbc)
        {
            _dbc = dbc;
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
        }

        public async Task<OverridesResponse> GetAsync(Guid userId)
        {
            var overrides = await _dbc.AvailabilityOverrides
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return new OverridesResponse
            {
                AvailabilityOverrides = overrides
            };
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
