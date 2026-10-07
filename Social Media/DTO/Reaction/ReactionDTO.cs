using Social_Media.Enums;

namespace Social_Media.DTO.Reaction
{
    public class ReactionDTO
    {
    }
    public enum ReactionResult
    {
        Added,
        Removed,
        Updated
    }

    public class ReactionRequest
    {
        public ReactionType ReactionType { get; set; }
    }
}
