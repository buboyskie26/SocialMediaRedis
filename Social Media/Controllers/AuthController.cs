using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media.DTO;
using Social_Media.DTO.Auth;
using Social_Media.Exceptions;
using Social_Media.Repository.UserRepository;
using Social_Media.Service.AuthService;

namespace Social_Media.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService,
            ILogger<AuthController> logger
            )
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("manualLogin")]
        public async Task<IActionResult> ManualLogin(LoginDTO loginDto)
        {
            try
            {
                var authResponse = await _authService.ManualLoginAsync(loginDto);

                _logger.LogInformation(
                    "User {EmpCode} successfully manual logged in with role {Role}",
                    authResponse.Email,
                    authResponse?.Role ?? "User");

                return Ok(new
                {
                    success = true,
                    user = authResponse,
                    message = "Login successful"
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
