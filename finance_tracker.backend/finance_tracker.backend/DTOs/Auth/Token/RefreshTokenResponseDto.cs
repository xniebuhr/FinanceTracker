namespace finance_tracker.backend.DTOs.Auth.Token
{
    public class RefreshTokenResponseDto
    {
        public required string AccessToken { get; set; }

        public required DateTime ExpiresAt { get; set; }

        public required string RefreshToken { get; set; }
    }
}
