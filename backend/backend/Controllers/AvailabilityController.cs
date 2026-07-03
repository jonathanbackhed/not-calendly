using backend.Extensions;
using backend.Models.DTOs.Requests;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/me/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailability()
        {
            var userId = User.GetUserId();

            var availabilityRules = await _availabilityService.GetAsync(userId);

            return Ok(availabilityRules);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailability(AvailabilityRequest request)
        {
            var userId = User.GetUserId();

            var rule = await _availabilityService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetAvailability), rule);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAvailability(Guid id, AvailabilityRequest request)
        {
            var userId = User.GetUserId();

            var rule = await _availabilityService.UpdateAsync(userId, id, request);

            return Ok(rule);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAvailability(Guid id)
        {
            var userId = User.GetUserId();

            await _availabilityService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}
