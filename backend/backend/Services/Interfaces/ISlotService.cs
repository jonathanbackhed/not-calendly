namespace backend.Services.Interfaces
{
    public interface ISlotService
    {
        Task<IEnumerable<DateOnly>> GetAvailableDatesForMonthAsync(string userSlug, string eventTypeSlug, int year, int month);
    }
}
