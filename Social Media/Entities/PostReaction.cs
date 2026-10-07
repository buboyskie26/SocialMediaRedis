using Social_Media.Enums;

namespace Social_Media.Entities
{
    public class PostReaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public ReactionType ReactionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; } = null!;
        public Post Post { get; set; } = null!;
    }
}
