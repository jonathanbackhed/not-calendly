using backend.Data;
using backend.Enums;
using backend.Exceptions;
using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;
using backend.Models.Entities;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _dbc;
        private readonly ILogger<BookingService> _logger;

        public BookingService(AppDbContext dbc, ILogger<BookingService> logger)
        {
            _dbc = dbc;
            _logger = logger;
        }

        public async Task<BookingResponse> CancelBooking(string cancelToken)
        {
            var booking = await _dbc.Bookings.FirstOrDefaultAsync(r => r.CancelToken == cancelToken);
            if (booking is null)
                throw new NotFoundException("Booking not found");

            if (booking.Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel already cancelled booking.");

            if (booking.StartsAt < DateTime.UtcNow)
                throw new InvalidOperationException("Cannot cancel a booking in the past.");

            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;

            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Booking {BookingId} canceled with correct token", booking.Id);

            return new BookingResponse(
                booking.GuestName,
                booking.GuestEmail,
                booking.GuestPhone,
                booking.StartsAt,
                booking.EndsAt,
                booking.Status.ToString(),
                booking.CancelToken,
                booking.Notes
            );
        }

        public async Task<BookingResponse> CreateBooking(BookingRequest request, string userSlug, string eventTypeSlug, DateTime startsAt)
        {
            var eventType = await _dbc.EventTypes.FirstOrDefaultAsync(r => r.Slug == eventTypeSlug && r.User!.Slug == userSlug);
            if (eventType is null)
                throw new NotFoundException("Event type not found");

            var booking = new Booking
            {
                UserId = eventType.UserId,
                EventTypeId = eventType.Id,
                GuestName = request.Name,
                GuestEmail = request.Email,
                GuestPhone = request.Phone,
                StartsAt = startsAt,
                EndsAt = startsAt.AddMinutes(eventType.DurationMinutes),
                Status = BookingStatus.Pending,
                CancelToken = Guid.NewGuid().ToString("N"),
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _dbc.Bookings.Add(booking);
            await _dbc.SaveChangesAsync();

            _logger.LogInformation("Booking {BookingId} created for event type id {EventTypeId}", booking.Id, eventType.Id);

            return new BookingResponse(
                booking.GuestName,
                booking.GuestEmail,
                booking.GuestPhone,
                booking.StartsAt,
                booking.EndsAt,
                booking.Status.ToString(),
                booking.CancelToken,
                booking.Notes
            );
        }
    }
}
