namespace finance_tracker.backend.DTOs.Auth.Token
{
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string RefreshToken { get; set; }
    }
}
