using Social_Media.Entities;
using Social_Media.Enums;

namespace Social_Media.DTO.Post
{
    public class PostDTO
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public string? PostImage { get; set; } = null;
        public PostVisibility Visibility { get; set; } = PostVisibility.Public;
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsEdited { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
    }

    public class PostFeedDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public string? PostImage { get; set; } = null;
        public PostVisibility Visibility { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //public List<PostReactionDTO>? PostReactionDTO { get; set; } = null;
        //public List<PostReactionDTO>? ReactionLike { get; set; } = null;
        public Dictionary<ReactionType, int>? ReactionsByTypeCount { get; set; }
        public int CommentsCount { get; set; }
        public int ReactionsCount { get; set; }
    }
    //
    //
    public class PostReactionDTO
    {
        public ReactionType ReactionTypeEnum { get; set; }
        public string ReactionTypeConversation => ReactionTypeEnum switch
        {
            ReactionType.Like => "Like",
            ReactionType.Love => "Love",
            ReactionType.Haha => "Haha",
            ReactionType.Wow => "Wow",
            ReactionType.Sad => "Sad",
            ReactionType.Angry => "Angry",
            _ => string.Empty
        };
        public int PostReactorId { get; set; }
        public string PostReactorUserName { get; set; } = string.Empty;
    }
    public class PostFeedTableDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public string? PostImage { get; set; } = null;
        public PostVisibility Visibility { get; set; } = PostVisibility.Public;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PostReactionDTO>? PostReactionDTO { get; set; } = null;
        public int CommentsCount { get; set; }
    }
    //
    public class PostReactionParams
    {
        public int PostId { get; set; }
        // Like, Love, Wow, Haha, Sad, Angry
        public ReactionType ReactionType { get; set; }
        //public string ReactionType { get; set; } = string.Empty;

    }
    public class PostReactorDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public ReactionType ReactionTypeEnum { get; set; }
    }
}
