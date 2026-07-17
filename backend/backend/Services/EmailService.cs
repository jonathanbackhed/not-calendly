using backend.Data;
using backend.Models.DTOs.Responses;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Resend;

namespace backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly AppDbContext _dbc;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(IResend resend, AppDbContext dbc, ILogger<EmailService> logger, IConfiguration config)
        {
            _resend = resend;
            _dbc = dbc;
            _logger = logger;
            _config = config;
        }

        public async Task SendCancellationEmail(string cancelToken)
        {
            var booking = await _dbc.Bookings.FirstOrDefaultAsync(r => r.CancelToken == cancelToken);
            if (booking is null)
                return;

            var eventType = await _dbc.EventTypes.FirstOrDefaultAsync(r => r.Id == booking.EventTypeId);
            if (eventType is null)
                return;

            var htmlBody = $"""
                <div style="font-family: sans-serif; background: #f4f4f5; padding: 24px; max-width: 480px; margin: 0 auto; background: #fff; border-radius: 8px; padding: 32px;">
                    <h2>Booking cancelled!</h2>
                    <p>Hi {booking.GuestName},</p>
                    <p>Your booking for <strong>{eventType.Title}</strong> at {booking.StartsAt} UTC has been cancelled.</p>
                </div>
            """;

            var emailSent = await SendEmail(booking.GuestEmail, "Booking cancelled", htmlBody);
            if (emailSent)
                _logger.LogInformation("Booking cancellation email sent successfully for booking {BookingId}", booking.Id);
        }

        public async Task SendConfirmationEmail(BookingResponse bookingResponse)
        {
            var booking = await _dbc.Bookings.FirstOrDefaultAsync(r => r.CancelToken == bookingResponse.CancelToken);
            if (booking is null)
                return;

            var eventType = await _dbc.EventTypes.FirstOrDefaultAsync(r => r.Id == booking.EventTypeId);
            if (eventType is null)
                return;

            var cancelBookingUrl = $"{_config["Frontend"]}/api/public/bookings/cancel/{bookingResponse.CancelToken}";

            var htmlBody = $"""
                <div style="font-family: sans-serif; background: #f4f4f5; padding: 24px; max-width: 480px; margin: 0 auto; background: #fff; border-radius: 8px; padding: 32px;">
                    <h2>Booking confirmed!🎉</h2>
                    <p>Hi {booking.GuestName},</p>
                    <p>Your booking for <strong>{eventType.Title}</strong> is confirmed for:</p>
                    <p style="font-size:18px;"><strong>{booking.StartsAt} UTC</strong></p>
                    <p>Type: {eventType.LocationType}</p>
                    <p>Where: {eventType.LocationValue}</p>
                    <p>Duration: {eventType.DurationMinutes} minutes</p>
                    <span>Wish to cancel your booking? <a href="{cancelBookingUrl}" style="color:#888;">Cancel booking</a></span>
                </div>
            """;

            var emailSent = await SendEmail(bookingResponse.Email, "Booking confirmation", htmlBody);
            if (emailSent)
                _logger.LogInformation("Booking confirmation email sent successfully for booking {BookingId}", booking.Id);
        }

        private async Task<bool> SendEmail(string email, string subject, string body)
        {
            var resp = await _resend.EmailSendAsync(new EmailMessage
            {
                From = _config["SenderEmail"]!,
                To = email,
                Subject = subject,
                HtmlBody = body
            });

            if (resp is null || !resp.Success)
                return false;

            return true;
        }
    }
}
