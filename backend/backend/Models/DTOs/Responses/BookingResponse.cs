using backend.Enums;

namespace backend.Models.DTOs.Responses
{
    public record BookingResponse(
        string Name,
        string Email,
        string? Phone,
        DateTime StartsAt,
        DateTime EndsAt,
        string Status,
        string CancelToken,
        string? Notes
        );
}
