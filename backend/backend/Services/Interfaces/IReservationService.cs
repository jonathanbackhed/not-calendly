namespace backend.Services.Interfaces
{
    public interface IReservationService
    {
        Task<string> AddReservationAsync(string userSlug, string eventTypeSlug, DateTime startsAt);
        Task RemoveReservationAsync(string userSlug, string eventTypeSlug, DateTime startsAt, string token);
    }
}
