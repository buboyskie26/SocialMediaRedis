using Social_Media.Enums;

namespace Social_Media.Entities
{
    public class Friendship
    {
        public int Id { get; set; }
        public int RequesterId { get; set; } // Who sent the friend request
        public int AddresseeId { get; set; } // Who received the friend request

        public FriendshipStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        // Navigation properties
        public User Requester { get; set; } = null!;
        public User Addressee { get; set; } = null!;
    }
}
