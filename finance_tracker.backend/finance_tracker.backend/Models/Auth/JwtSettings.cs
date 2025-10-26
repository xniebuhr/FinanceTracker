namespace finance_tracker.backend.Models.Auth
{
    public class JwtSettings
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpiresMinutes { get; set; } = 60;

        public int RefreshTokenDays { get; set; } = 7;
    }
}