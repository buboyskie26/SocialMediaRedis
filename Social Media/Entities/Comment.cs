namespace Social_Media.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int AuthorId { get; set; }
        // Self referencing
        public int? ParentCommentId { get; set; }

        public required string Content { get; set; } = string.Empty;
        public string? CommentImage { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsEdited { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Post Post { get; set; } = null!;
        public User Author { get; set; } = null!;
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
