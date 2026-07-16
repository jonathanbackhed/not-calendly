using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;

namespace backend.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBooking(BookingRequest request, string userSlug, string eventTypeSlug, DateTime startsAt);
        Task<BookingResponse> CancelBooking(string cancelToken);
    }
}
