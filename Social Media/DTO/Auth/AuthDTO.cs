using System.ComponentModel.DataAnnotations;

namespace Social_Media.DTO.Auth
{
    public class AuthDTO
    {
    }
    //
    public class LoginDTO
    {
        //[Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;

    }

    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
