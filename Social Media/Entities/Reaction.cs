using Social_Media.Enums;

namespace Social_Media.Entities
{
    // Polymorphic
    public class Reaction
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        // Post=0, Comment=1
        public ReactionTargetType TargetType { get; set; }

        public int TargetId { get; set; }

        public ReactionType ReactionType { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

    }
}
