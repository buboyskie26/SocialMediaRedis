namespace Social_Media.Service.CurrentService
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string? Email { get; }
        string? Username { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}
