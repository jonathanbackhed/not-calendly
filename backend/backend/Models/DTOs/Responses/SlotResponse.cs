namespace backend.Models.DTOs.Responses
{
    public record SlotResponse
    {
        public DateTime StartsAt { get; init; }
        public DateTime EndsAt { get; init; }
    }
}
