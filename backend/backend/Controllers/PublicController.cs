using backend.Models.DTOs.Requests;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicController : ControllerBase
    {
        private readonly ISlotService _slotService;
        private readonly IReservationService _reservationService;
        private readonly IEventTypeService _eventTypeService;
        private readonly IBookingService _bookingService;

        public PublicController(ISlotService slotService, IReservationService reservationService, IEventTypeService eventTypeService, IBookingService bookingService)
        {
            _slotService = slotService;
            _reservationService = reservationService;
            _eventTypeService = eventTypeService;
            _bookingService = bookingService;
        }

        // AVAILABILITY
        [HttpGet("{userSlug}")]
        public async Task<IActionResult> GetEventTypes([FromRoute] string userSlug)
        {
            var eventTypes = await _eventTypeService.GetFromSlugAsync(userSlug);

            return Ok(eventTypes);
        }

        [HttpGet("{userSlug}/{eventTypeSlug}/availabledays")]
        public async Task<IActionResult> GetAvailableDatesForMonth([FromRoute] string userSlug, [FromRoute] string eventTypeSlug, [FromQuery] int year, [FromQuery] int month)
        {
            var availableDates = await _slotService.GetAvailableDatesForMonthAsync(userSlug, eventTypeSlug, year, month);

            return Ok(availableDates);
        }

        [HttpGet("{userSlug}/{eventTypeSlug}/availableslots")]
        public async Task<IActionResult> GetAvailableSlotsForDate([FromRoute] string userSlug, [FromRoute] string eventTypeSlug, [FromQuery] DateOnly date)
        {
            var availableSlots = await _slotService.GetAvailableSlotsForDateAsync(userSlug, eventTypeSlug, date);

            return Ok(availableSlots);
        }

        // RESERVE SLOT
        [HttpPost("{userSlug}/{eventTypeSlug}/reserveslot")]
        public async Task<IActionResult> ReserveSlot([FromRoute] string userSlug, [FromRoute] string eventTypeSlug, [FromQuery] DateTime startsAt)
        {
            var token = await _reservationService.AddReservationAsync(userSlug, eventTypeSlug, startsAt);
            Response.Cookies.Append("reservation_token", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            });

            return Ok();
        }

        [HttpDelete("{userSlug}/{eventTypeSlug}/releaseslot")]
        public async Task<IActionResult> ReleaseSlot([FromRoute] string userSlug, [FromRoute] string eventTypeSlug, [FromQuery] DateTime startsAt)
        {
            var reservationToken = Request.Cookies.TryGetValue("reservation_token", out var token) ? token : null;
            if (reservationToken is null)
                return BadRequest("Reservation token missing.");

            await _reservationService.RemoveReservationAsync(userSlug, eventTypeSlug, startsAt, reservationToken);

            Response.Cookies.Delete("reservation_token");

            return Ok();
        }

        // BOOKINGS
        [HttpPost("{userSlug}/{eventTypeSlug}/bookings")]
        public async Task<IActionResult> BookSlot([FromBody] BookingRequest request, [FromRoute] string userSlug, [FromRoute] string eventTypeSlug, [FromQuery] DateTime startsAt)
        {
            var reservationToken = Request.Cookies.TryGetValue("reservation_token", out var token) ? token : "";

            var slotAvailable = await _slotService.IsSlotAvailableAsync(userSlug, eventTypeSlug, startsAt, reservationToken);
            if (!slotAvailable)
            {
                return BadRequest();
            }

            var booking = await _bookingService.CreateBooking(request, userSlug, eventTypeSlug, startsAt);

            if (!string.IsNullOrEmpty(reservationToken))
            {
                await _reservationService.RemoveReservationAsync(userSlug, eventTypeSlug, startsAt, reservationToken);
                Response.Cookies.Delete("reservation_token");
            }

            // Send confirmation email to host & user

            return Ok(booking);
        }

        [HttpPut("bookings/cancel/{cancelToken}")]
        public async Task<IActionResult> CancelSlot(string cancelToken)
        {
            var booking = await _bookingService.CancelBooking(cancelToken);

            // Send cancellation email to host & user

            return Ok(booking);
        }
    }
}
