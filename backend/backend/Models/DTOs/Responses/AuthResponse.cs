namespace backend.Models.DTOs.Responses
{
    public record AuthResponse(string AccessToken, string RefreshToken);
}
