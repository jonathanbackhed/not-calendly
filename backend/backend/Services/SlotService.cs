using backend.Cache;
using backend.Data;
using backend.Enums;
using backend.Exceptions;
using backend.Models.DTOs.Responses;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class SlotService : ISlotService
    {
        private readonly AppDbContext _dbc;
        private readonly ReservationCache _cache;

        public SlotService(AppDbContext dbc, ReservationCache cache)
        {
            _dbc = dbc;
            _cache = cache;
        }

        public async Task<IEnumerable<DateOnly>> GetAvailableDatesForMonthAsync(string userSlug, string eventTypeSlug, int year, int month)
        {
            var eventType = await _dbc.EventTypes.FirstOrDefaultAsync(r => r.Slug == eventTypeSlug && r.User!.Slug == userSlug && r.IsActive);
            if (eventType is null)
                throw new NotFoundException("Event type not found");

            var availabilityRules = await _dbc.AvailabilityRules
                .Where(r => r.UserId == eventType.UserId && r.IsActive)
                .ToListAsync();
            if (!availabilityRules.Any())
                return Enumerable.Empty<DateOnly>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var lastDay = new DateOnly(year, month, 1).AddMonths(1).AddDays(-1);

            var overrides = await _dbc.AvailabilityOverrides
                .Where(r => r.UserId == eventType.UserId && r.OverrideDate >= today && r.OverrideDate <= lastDay)
                .ToListAsync();

            var maxDate = today.AddDays(eventType.MaxDaysInAdvance);
            var availableDates = new List<DateOnly>();

            for (var date = today; date <= lastDay; date = date.AddDays(1))
            {
                if (date > maxDate)
                    continue;

                var dayOfWeek = (int)date.DayOfWeek;
                var rule = availabilityRules.FirstOrDefault(r => r.DayOfWeek == dayOfWeek);
                if (rule is null)
                    continue;

                var overrideRule = overrides.FirstOrDefault(r => r.OverrideDate == date);
                if (overrideRule is not null && overrideRule.IsBlocked)
                    continue;

                availableDates.Add(date);
            }

            return availableDates;
        }

        public async Task<IEnumerable<SlotResponse>> GetAvailableSlotsForDateAsync(string userSlug, string eventTypeSlug, DateOnly date)
        {
            var eventType = await _dbc.EventTypes
                .FirstOrDefaultAsync(r => r.Slug == eventTypeSlug && r.User!.Slug == userSlug && r.IsActive);
            if (eventType is null)
                throw new NotFoundException("Event type not found");

            var availabilityRule = await _dbc.AvailabilityRules
                .FirstOrDefaultAsync(r => r.UserId == eventType.UserId && r.IsActive && r.DayOfWeek == (int)date.DayOfWeek);
            if (availabilityRule is null)
                return Enumerable.Empty<SlotResponse>();

            var availabilityOverride = await _dbc.AvailabilityOverrides
                .FirstOrDefaultAsync(r => r.UserId == eventType.UserId && r.OverrideDate == date);
            if (availabilityOverride is not null && availabilityOverride.IsBlocked)
                return Enumerable.Empty<SlotResponse>();

            var startTime = availabilityOverride?.StartTime ?? availabilityRule.StartTime;
            var endTime = availabilityOverride?.EndTime ?? availabilityRule.EndTime;

            var existingBookings = await _dbc.Bookings
                .Where(r => r.UserId == eventType.UserId && DateOnly.FromDateTime(r.StartsAt) == date && r.Status != BookingStatus.Cancelled)
                .ToListAsync();

            var slots = new List<SlotResponse>();
            var current = startTime;

            while (current.AddMinutes(eventType.DurationMinutes) <= endTime)
            {
                var slotStart = date.ToDateTime(current, DateTimeKind.Utc);
                var slotEnd = slotStart.AddMinutes(eventType.DurationMinutes);

                var bufferStart = slotStart.AddMinutes(-eventType.BufferBeforeMinutes);
                var bufferEnd = slotEnd.AddMinutes(eventType.BufferAfterMinutes);

                var hasConflict = existingBookings.Any(r => bufferStart < r.EndsAt && bufferEnd > r.StartsAt);
                var isReserved = _cache.IsReserved(eventType.UserId, eventType.Id, slotStart);

                if (!hasConflict && !isReserved)
                {
                    slots.Add(new SlotResponse
                    {
                        StartsAt = slotStart,
                        EndsAt = slotEnd
                    });
                }

                current = current.AddMinutes(eventType.DurationMinutes);
            }

            return slots;
        }

        public async Task<bool> IsSlotAvailableAsync(string userSlug, string eventTypeSlug, DateTime startsAt, string reservationToken)
        {
            var eventType = await _dbc.EventTypes
                .FirstOrDefaultAsync(r => r.Slug == eventTypeSlug && r.User!.Slug == userSlug && r.IsActive);
            if (eventType is null)
                throw new NotFoundException("Event type not found");

            if (startsAt < DateTime.UtcNow)
                return false;

            var date = DateOnly.FromDateTime(startsAt);
            var endsAt = startsAt.AddMinutes(eventType.DurationMinutes);
            var slotStartTime = TimeOnly.FromDateTime(startsAt);
            var slotEndTime = TimeOnly.FromDateTime(endsAt);

            var availabilityRule = await _dbc.AvailabilityRules
                .FirstOrDefaultAsync(r => r.UserId == eventType.UserId && r.IsActive && r.DayOfWeek == (int)startsAt.DayOfWeek);
            if (availabilityRule is null)
                return false;

            if (slotStartTime < availabilityRule.StartTime || slotEndTime > availabilityRule.EndTime)
                return false;

            var availabilityOverride = await _dbc.AvailabilityOverrides
                .FirstOrDefaultAsync(r => r.UserId == eventType.UserId && r.OverrideDate == date);
            if (availabilityOverride is not null)
            {
                if (availabilityOverride.IsBlocked)
                    return false;

                if (availabilityOverride.StartTime.HasValue && availabilityOverride.EndTime.HasValue)
                {
                    if (slotStartTime < availabilityOverride.StartTime.Value || slotEndTime > availabilityOverride.EndTime.Value)
                        return false;
                }
            }

            var bufferStart = startsAt.AddMinutes(-eventType.BufferBeforeMinutes);
            var bufferEnd = endsAt.AddMinutes(eventType.BufferAfterMinutes);

            var hasConflict = await _dbc.Bookings
                .AnyAsync(r => r.EventTypeId == eventType.Id
                    && r.Status != BookingStatus.Cancelled
                    && r.StartsAt < bufferEnd
                    && r.EndsAt > bufferStart);
            if (hasConflict)
                return false;

            if (!_cache.IsReservedWithToken(eventType.UserId, eventType.Id, startsAt, reservationToken))
                return false;

            return true;
        }
    }
}
