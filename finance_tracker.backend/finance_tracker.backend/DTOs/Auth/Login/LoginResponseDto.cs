namespace finance_tracker.backend.DTOs.Auth.Login
{
    public class LoginResponseDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }
        
        public string AccessToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string RefreshToken { get; set; }
    }
}