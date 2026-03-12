namespace finance_tracker.backend.DTOs.Users
{
    public class UserResponseDto
    {
        public required string Id { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
