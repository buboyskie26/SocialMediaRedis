using Social_Media.Enums;

namespace Social_Media.Entities
{
    public class CommentReaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CommentId { get; set; }
        public ReactionType ReactionType { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
        public Comment Comment { get; set; } = null!;
    }
}
