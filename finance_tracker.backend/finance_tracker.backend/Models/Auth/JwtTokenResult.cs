namespace finance_tracker.backend.Models.Auth
{
    public class JwtTokenResult
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
