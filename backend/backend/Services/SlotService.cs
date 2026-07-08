using backend.Data;
using backend.Exceptions;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class SlotService : ISlotService
    {
        private readonly AppDbContext _dbc;

        public SlotService(AppDbContext dbc)
        {
            _dbc = dbc;
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
    }
}
