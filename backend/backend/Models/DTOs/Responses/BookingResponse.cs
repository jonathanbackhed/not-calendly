using backend.Enums;

namespace backend.Models.DTOs.Responses
{
    public record BookingResponse(
        string Name,
        string Email,
        string? Phone,
        DateTime StartsAt,
        DateTime EndsAt,
        BookingStatus Status,
        string CancelToken,
        string? Notes
        );
}
