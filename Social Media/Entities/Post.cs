using Social_Media.Enums;

namespace Social_Media.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public required string Content { get; set; } = string.Empty;

        public string? PostImage { get; set; } = null;

        public PostVisibility Visibility { get; set; } = PostVisibility.Public;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsEdited { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public User Author { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
        //public ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();


        // To prevent read date aggregation for Post/Reaction Count.
        public int CommentsCount { get; set; } = 0;
        public int ReactionsCount { get; set; } = 0;
    }
}