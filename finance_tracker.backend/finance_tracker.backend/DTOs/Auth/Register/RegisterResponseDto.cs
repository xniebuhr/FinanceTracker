namespace finance_tracker.backend.DTOs.Auth.Register
{
    public class RegisterResponseDto
    {
        public required string Id { get; set; }

        public required string Username { get; set; }

        public required string FirstName { get; set; }

        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }

        public required DateTime ExpiresAt { get; set; }
    }
}