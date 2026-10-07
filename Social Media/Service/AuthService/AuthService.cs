using Social_Media.DTO.Auth;
using Social_Media.DTO.User;
using Social_Media.Entities;
using Social_Media.Exceptions;
using Social_Media.Repository.UserRepository;
using Social_Media.Service.JwtService;

namespace Social_Media.Service.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
                _userRepository = userRepository;
                _jwtService = jwtService;
        }
        public async Task<AuthResponseDTO> ManualLoginAsync(LoginDTO loginDTO)
        {
            // Check if user exists
            var user = await _userRepository.GetSingleAsync(w => w.Email == loginDTO.Email);

            if (user == null)
                throw new NotFoundException("User not found");

            var userDTO = new UserDTO
            {
                AccessLevel = "",
                Email = user.Email,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                DisplayName = user.DisplayName,
                Id = user.Id
            };
            //
            var jwtResult = await _jwtService.GenerateToken(userDTO);

            var expiresAt = DateTime.Now.AddDays(7);

            return new AuthResponseDTO
            {
                Token = jwtResult.token,
                Role = jwtResult.role,
                DisplayName = userDTO.DisplayName!,
                Email = userDTO.Email,
                Username = userDTO.Username
                //Role = userAccess == null ? "User" : userAccess.AccessType,
            };
        }
    
        //
        //
    
    }
}
