using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

namespace Social_Media.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string PasswordHash { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}
