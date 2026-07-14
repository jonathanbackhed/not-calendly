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
    public class EventTypeService : IEventTypeService
    {
        private readonly AppDbContext _dbc;
        private readonly ILogger<EventTypeService> _logger;

        public EventTypeService(AppDbContext dbc, ILogger<EventTypeService> logger)
        {
            _dbc = dbc;
            _logger = logger;
        }

        public async Task<EventTypeResponse> CreateAsync(Guid userId, EventTypeRequest request)
        {
            var exists = await _dbc.EventTypes.AnyAsync(e => e.UserId == userId && e.Slug == request.Slug);
            if (exists)
                throw new ConflictException("An event type with this slug already exists.");

            if (string.IsNullOrWhiteSpace(request.LocationValue))
                throw new ValidationException("LocationValue is required.");

            var eventType = new EventType
            {
                UserId = userId,
                Title = request.Title,
                Slug = request.Slug,
                DurationMinutes = request.DurationMinutes,
                LocationType = request.LocationType,
                LocationValue = request.LocationValue,
                BufferBeforeMinutes = request.BufferBeforeMinutes,
                BufferAfterMinutes = request.BufferAfterMinutes,
                MaxDaysInAdvance = request.MaxDaysInAdvance,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _dbc.EventTypes.Add(eventType);
            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Event type {Slug} created for user {UserId} ", eventType.Slug, userId);

            return new EventTypeResponse
            {
                Id = eventType.Id,
                Title = eventType.Title,
                Slug = eventType.Slug,
                DurationMinutes = eventType.DurationMinutes,
                LocationType = eventType.LocationType,
                LocationValue = eventType.LocationValue,
                BufferBeforeMinutes = eventType.BufferBeforeMinutes,
                BufferAfterMinutes = eventType.BufferAfterMinutes,
                MaxDaysInAdvance = eventType.MaxDaysInAdvance,
                IsActive = eventType.IsActive,
                CreatedAt = eventType.CreatedAt
            };
        }

        public async Task DeleteAsync(Guid userId, Guid eventTypeId)
        {
            var deleted = await _dbc.EventTypes
                .Where(r => r.Id == eventTypeId && r.UserId == userId)
                .ExecuteDeleteAsync();

            if (deleted == 0)
                throw new NotFoundException("Event type not found.");

            _logger.LogInformation("Event type {EventTypeId} deleted by user {UserId}", eventTypeId, userId);
        }

        public async Task<IEnumerable<EventTypeResponse>> GetAsync(Guid userId)
        {
            var eventTypes = await _dbc.EventTypes
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Select(r => new EventTypeResponse
                {
                    Id = r.Id,
                    Title = r.Title,
                    Slug = r.Slug,
                    DurationMinutes = r.DurationMinutes,
                    LocationType = r.LocationType,
                    LocationValue = r.LocationValue,
                    BufferBeforeMinutes = r.BufferBeforeMinutes,
                    BufferAfterMinutes = r.BufferAfterMinutes,
                    MaxDaysInAdvance = r.MaxDaysInAdvance,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return eventTypes;
        }

        public async Task<EventTypeResponse> UpdateAsync(Guid userId, Guid eventTypeId, EventTypeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LocationValue))
                throw new ValidationException("LocationValue is required.");

            var eventType = await _dbc.EventTypes.FirstOrDefaultAsync(r => r.Id == eventTypeId && r.UserId == userId);
            if (eventType is null)
                throw new NotFoundException("Event type not found.");

            if (eventType.Slug != request.Slug)
                throw new ValidationException("Slug cannot be changed after creation.");

            eventType.Title = request.Title;
            eventType.DurationMinutes = request.DurationMinutes;
            eventType.LocationType = request.LocationType;
            eventType.LocationValue = request.LocationValue;
            eventType.BufferBeforeMinutes = request.BufferBeforeMinutes;
            eventType.BufferAfterMinutes = request.BufferAfterMinutes;
            eventType.MaxDaysInAdvance = request.MaxDaysInAdvance;
            eventType.IsActive = request.IsActive;
            eventType.UpdatedAt = DateTime.UtcNow;

            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Event type {EventTypeId} updated by user {UserId}", eventTypeId, userId);

            return new EventTypeResponse
            {
                Id = eventType.Id,
                Title = eventType.Title,
                Slug = eventType.Slug,
                DurationMinutes = eventType.DurationMinutes,
                LocationType = eventType.LocationType,
                LocationValue = eventType.LocationValue,
                BufferBeforeMinutes = eventType.BufferBeforeMinutes,
                BufferAfterMinutes = eventType.BufferAfterMinutes,
                MaxDaysInAdvance = eventType.MaxDaysInAdvance,
                IsActive = eventType.IsActive,
                CreatedAt = eventType.CreatedAt
            };
        }
    }
}
