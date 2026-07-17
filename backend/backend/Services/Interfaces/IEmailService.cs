using backend.Models.DTOs.Responses;

namespace backend.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationEmail(BookingResponse bookingResponse);
        Task SendCancellationEmail(string cancelToken);
    }
}
