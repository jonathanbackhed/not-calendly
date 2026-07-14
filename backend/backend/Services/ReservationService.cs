using backend.Cache;
using backend.Data;
using backend.Exceptions;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ReservationCache _cache;
        private readonly AppDbContext _dbc;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(ReservationCache cache, AppDbContext dbc, ILogger<ReservationService> logger)
        {
            _cache = cache;
            _dbc = dbc;
            _logger = logger;
        }

        public async Task<string> AddReservationAsync(string userSlug, string eventTypeSlug, DateTime startsAt)
        {
            var eventType = await _dbc.EventTypes
                .FirstOrDefaultAsync(r => r.Slug == eventTypeSlug && r.User!.Slug == userSlug && r.IsActive);
            if (eventType is null)
                throw new NotFoundException("Event type not found.");

            if (startsAt < DateTime.UtcNow)
                throw new ValidationException("Cannot reserve a slot in the past");

            if (_cache.IsReserved(eventType.UserId, eventType.Id, startsAt))
            {
                _logger.LogWarning("Slot reservation attempt for already reserved slot. Event type {EventTypeId} at {StartsAt}", eventType.Id, startsAt);
                throw new ConflictException("This slot is already reserved.");
            }
                

            var token = _cache.Reserve(eventType.UserId, eventType.Id, startsAt);

            _logger.LogInformation("Slot reserved for event type {EventTypeId} at {StartsAt}", eventType.Id, startsAt);

            return token;
        }

        public async Task RemoveReservationAsync(string userSlug, string eventTypeSlug, DateTime startsAt, string token)
        {
            var eventType = await _dbc.EventTypes
                .FirstOrDefaultAsync(r => r.Slug == eventTypeSlug && r.User!.Slug == userSlug && r.IsActive);
            if (eventType is null)
                throw new NotFoundException("Event type not found.");

            if (startsAt < DateTime.UtcNow)
                throw new ValidationException("Cannot reserve a slot in the past");

            if (!_cache.IsReserved(eventType.UserId, eventType.Id, startsAt))
            {
                _logger.LogWarning("Release attempt for non-reserved slot. Event type {EventTypeId} at {StartsAt}", eventType.Id, startsAt);
                throw new ConflictException("This slot is not reserved.");
            }

            var released = _cache.Release(eventType.UserId, eventType.Id, startsAt, token);
            if (!released)
            {
                _logger.LogWarning("Invalid reservation token for event type {EventTypeId} at {StartsAt}", eventType.Id, startsAt);
                throw new ValidationException("Invalid reservation token");
            }

            _logger.LogInformation("Slot reservation release for event type {EventTypeId} at {StartsAt}", eventType.Id, startsAt);
        }
    }
}
