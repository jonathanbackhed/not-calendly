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
    public class AvailabilityService : IAvailabilityService
    {
        private readonly AppDbContext _dbc;
        private readonly ILogger<AvailabilityService> _logger;

        public AvailabilityService(AppDbContext dbc, ILogger<AvailabilityService> logger)
        {
            _dbc = dbc;
            _logger = logger;
        }

        public async Task<AvailabilityResponse> CreateAsync(Guid userId, AvailabilityRequest request)
        {
            var exists = await _dbc.AvailabilityRules.AnyAsync(r => r.UserId == userId && r.DayOfWeek == request.DayOfWeek);
            if (exists) 
                throw new ConflictException("Availability rule for this day already exists.");

            if (request.StartTime >= request.EndTime) 
                throw new ValidationException("Start time must be before end time.");

            var availabilityRule = new AvailabilityRule()
            {
                UserId = userId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _dbc.AvailabilityRules.Add(availabilityRule);
            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Availability rule created for user {UserId} on day {DayOfWeek}", userId, (DayOfWeek)availabilityRule.DayOfWeek);

            return new AvailabilityResponse
            {
                Id = availabilityRule.Id,
                DayOfWeek = availabilityRule.DayOfWeek,
                StartTime = availabilityRule.StartTime,
                EndTime = availabilityRule.EndTime,
                IsActive = availabilityRule.IsActive,
                CreatedAt = availabilityRule.CreatedAt
            };
        }

        public async Task DeleteAsync(Guid userId, Guid availabilityRuleId)
        {
            var deleted = await _dbc.AvailabilityRules
                .Where(r => r.Id == availabilityRuleId && r.UserId == userId)
                .ExecuteDeleteAsync();

            if (deleted == 0)
                throw new NotFoundException("Availability rule not found.");

            _logger.LogInformation("Availability rule {RuleId} deleted by user {UserId}", availabilityRuleId, userId);
        }

        public async Task<IEnumerable<AvailabilityResponse>> GetAsync(Guid userId)
        {
            var availabilityRules = await _dbc.AvailabilityRules
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Select(r => new AvailabilityResponse
                {
                    Id = r.Id,
                    DayOfWeek = r.DayOfWeek,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return availabilityRules;
        }

        public async Task<AvailabilityResponse> UpdateAsync(Guid userId, Guid availabilityRuleId, AvailabilityRequest request)
        {
            if (request.StartTime >= request.EndTime)
                throw new ValidationException("Start time must be before end time.");

            var rule = await _dbc.AvailabilityRules.FirstOrDefaultAsync(r => r.Id == availabilityRuleId && r.UserId == userId);
            if (rule is null)
                throw new NotFoundException("Availability rule not found.");

            rule.DayOfWeek = request.DayOfWeek;
            rule.StartTime = request.StartTime;
            rule.EndTime = request.EndTime;
            rule.IsActive = request.IsActive;
            rule.UpdatedAt = DateTime.UtcNow;

            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Availability rule {RuleId} updated by user {UserId}", availabilityRuleId, userId);

            return new AvailabilityResponse
            {
                Id = rule.Id,
                DayOfWeek = rule.DayOfWeek,
                StartTime = rule.StartTime,
                EndTime = rule.EndTime,
                IsActive = rule.IsActive,
                CreatedAt = rule.CreatedAt,
            };
        }
    }
}
