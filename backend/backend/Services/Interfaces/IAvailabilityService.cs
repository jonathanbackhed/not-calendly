using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;

namespace backend.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<AvailabilityResponse> GetAsync(Guid userId);
        Task<AvailabilityRuleResponse> CreateAsync(Guid userId, AvailabilityRequest request);
        Task<AvailabilityRuleResponse> UpdateAsync(Guid userId, Guid availabilityRuleId, AvailabilityRequest request);
        Task DeleteAsync(Guid userId, Guid availabilityRuleId);
    }
}
