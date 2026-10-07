namespace Social_Media.DTO.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string Username { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string AccessLevel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? DisplayName { get; set; }
    }
}
