using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;

namespace backend.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<IEnumerable<AvailabilityResponse>> GetAsync(Guid userId);
        Task<AvailabilityResponse> CreateAsync(Guid userId, AvailabilityRequest request);
        Task<AvailabilityResponse> UpdateAsync(Guid userId, Guid availabilityRuleId, AvailabilityRequest request);
        Task DeleteAsync(Guid userId, Guid availabilityRuleId);
    }
}
