using backend.Models.Entities;

namespace backend.Models.DTOs.Responses
{
    public record AvailabilityResponse
    {
        public IEnumerable<AvailabilityRule>? AvailabilityRules { get; init; }
    }
}
