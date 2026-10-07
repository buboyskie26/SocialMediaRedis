using Microsoft.IdentityModel.Tokens;
using Social_Media.DTO.User;
using Social_Media.Exceptions;
using Social_Media.Repository.UserAccessRepository;
using Social_Media.Repository.UserRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Social_Media.Service.JwtService
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IUserAccessRepository _userAccessRepository;

        public JwtService(IConfiguration configuration,
            IUserRepository userRepository, IUserAccessRepository userAccessRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _userAccessRepository = userAccessRepository;
        }
        public async Task<TokenResult> GenerateToken(UserDTO user)
        {
            var userAccess = await _userAccessRepository.GetSingleAsync(w=> w.UserId == user.Id);

            var userObject = await _userRepository.GetSingleAsync(w=> w.Email == user.Email);

            if (userObject == null)
                throw new NotFoundException("userObject not found.");

            var userAccessType = userAccess == null ? "User" : userAccess.AccessType;
            //var userAccessType = "User" ;
            //
            var claims = new List<Claim>
            {
                  new Claim(ClaimTypes.NameIdentifier, userObject.Id.ToString()),
                  new Claim(ClaimTypes.Email, user.Email),
                  new Claim(ClaimTypes.Name, user.Username),
                  new Claim(ClaimTypes.Role, userAccessType),
            };
            // Get the secret key from appsettings.json
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)
            );
            // Create signing credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var expiresAt = DateTime.Now.AddDays(7);

            // Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            // 
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //
            var returnedToken = tokenHandler.WriteToken(token);

            var tokenResult = new TokenResult(returnedToken, userAccessType);
            return tokenResult;
        }

        public string? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
