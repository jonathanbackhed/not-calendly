using backend.Models.DTOs.Requests;
using backend.Models.DTOs.Responses;

namespace backend.Services.Interfaces
{
    public interface IEventTypeService
    {
        Task<IEnumerable<EventTypeResponse>> GetAsync(Guid userId);
        Task<EventTypeResponse> CreateAsync(Guid userId, EventTypeRequest request);
        Task<EventTypeResponse> UpdateAsync(Guid userId, Guid eventTypeId, EventTypeRequest request);
        Task DeleteAsync(Guid userId, Guid eventTypeId);
    }
}
