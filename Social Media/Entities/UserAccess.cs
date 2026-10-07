namespace Social_Media.Entities
{
    public class UserAccess
    {
        public int UserAccessID { get; set; }
        public  int UserId { get; set; }
        public required string AccessType { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
