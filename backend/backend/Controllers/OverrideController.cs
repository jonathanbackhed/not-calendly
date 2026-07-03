using backend.Extensions;
using backend.Models.DTOs.Requests;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/me/[controller]s")]
    public class OverrideController : ControllerBase
    {
        private readonly IAvailabilityOverrideService _overrideService;

        public OverrideController(IAvailabilityOverrideService overrideService)
        {
            _overrideService = overrideService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOverrides()
        {
            var userId = User.GetUserId();

            var overrides = await _overrideService.GetAsync(userId);

            return Ok(overrides);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOverride(AvailabilityOverrideRequest request)
        {
            var userId = User.GetUserId();

            var overrideEntity = await _overrideService.CreateAsync(userId, request);

            return CreatedAtAction(nameof(GetOverrides), overrideEntity);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateOverride(Guid id, AvailabilityOverrideRequest request)
        {
            var userId = User.GetUserId();

            var overrideEntity = await _overrideService.UpdateAsync(userId, id, request);

            return Ok(overrideEntity);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteOverride(Guid id)
        {
            var userId = User.GetUserId();

            await _overrideService.DeleteAsync(userId, id);

            return NoContent();
        }
    }
}
