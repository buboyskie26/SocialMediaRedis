using Social_Media.DTO.Auth;

namespace Social_Media.Service.AuthService
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> ManualLoginAsync(LoginDTO loginDTO);
    }
}
