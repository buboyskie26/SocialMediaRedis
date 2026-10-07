using System.Security.Claims;

namespace Social_Media.Service.CurrentService
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int UserId
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?.User
                    ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                //return int.Parse(userIdString);
                return int.TryParse(userIdString, out var userId) ? userId : 0;
                //
                //return string.TryParse(userIdString, out var userId) ? userId : null;
            }
        }
        //public string? UserId =>
        //    _httpContextAccessor.HttpContext?
        //        .User?
        //        .FindFirstValue(ClaimTypes.NameIdentifier)
        //    is string id && !string.IsNullOrWhiteSpace(id)
        //        ? id
        //        : null;

        public string? Email => _httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.Email)?.Value;

        public string? Username => _httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.Name)?.Value;

        public string? Role => _httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.Role)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public bool IsAdmin => Role == "Admin";
        public bool IsUser => Role == "User";
    }
}
