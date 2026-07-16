using backend.Models.DTOs.Responses;

namespace backend.Services.Interfaces
{
    public interface ISlotService
    {
        Task<IEnumerable<DateOnly>> GetAvailableDatesForMonthAsync(string userSlug, string eventTypeSlug, int year, int month);
        Task<IEnumerable<SlotResponse>> GetAvailableSlotsForDateAsync(string userSlug, string eventTypeSlug, DateOnly date);
        Task<bool> IsSlotAvailableAsync(string userSlug, string eventTypeSlug, DateTime startsAt, string reservationToken);
    }
}
