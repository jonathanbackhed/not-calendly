using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicController : ControllerBase
    {
        private readonly ISlotService _slotService;

        public PublicController(ISlotService slotService)
        {
            _slotService = slotService;
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
    }
}
