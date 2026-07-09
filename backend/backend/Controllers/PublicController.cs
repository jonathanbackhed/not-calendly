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

        public PublicController(ISlotService slotService, IReservationService reservationService)
        {
            _slotService = slotService;
            _reservationService = reservationService;
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

            return Ok();
        }
    }
}
