using backend.Models.Entities;

namespace backend.Models.DTOs.Responses
{
    public record OverridesResponse
    {
        public IEnumerable<AvailabilityOverride>? AvailabilityOverrides { get; init; }
    }
}
