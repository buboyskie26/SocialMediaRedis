using Social_Media.DTO.User;

namespace Social_Media.Service.JwtService
{
    public interface IJwtService
    {
        Task<TokenResult> GenerateToken(UserDTO user);
        string? ValidateToken(string token);
    }

    public record TokenResult(string token, string role);
}
