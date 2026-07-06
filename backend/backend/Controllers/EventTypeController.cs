using backend.Extensions;
using backend.Models.DTOs.Requests;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/me/event-types")]
    public class EventTypeController : ControllerBase
    {
        private readonly IEventTypeService _eventTypeService;

        public EventTypeController(IEventTypeService eventTypeService)
        {
            _eventTypeService = eventTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventTypes()
        {
            var userId = User.GetUserId();

            var eventTypes = await _eventTypeService.GetAsync(userId);

            return Ok(eventTypes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEventType(EventTypeRequest request)
        {
            var userId = User.GetUserId();

            var eventType = await _eventTypeService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetEventTypes), eventType);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEventTypes(Guid id, EventTypeRequest request)
        {
            var userId = User.GetUserId();

            var eventType = await _eventTypeService.UpdateAsync(userId, id, request);

            return Ok(eventType);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEventTypes(Guid id)
        {
            var userId = User.GetUserId();

            await _eventTypeService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}
