using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;

namespace backend.Services.Interfaces
{
    public interface IAvailabilityOverrideService
    {
        Task<IEnumerable<AvailabilityOverrideResponse>> GetAsync(Guid userId);
        Task<AvailabilityOverrideResponse> CreateAsync(Guid userId, AvailabilityOverrideRequest request);
        Task<AvailabilityOverrideResponse> UpdateAsync(Guid userId, Guid overrideId, AvailabilityOverrideRequest request);
        Task DeleteAsync(Guid userId, Guid overrideId);
    }
}
