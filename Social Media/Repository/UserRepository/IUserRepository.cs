using Social_Media.DTO;
using Social_Media.DTO.Pagination;
using Social_Media.DTO.Post;
using Social_Media.DTO.Reaction;
using Social_Media.DTO.User;
using Social_Media.Entities;
using Social_Media.Enums;
using Social_Media.Repository.Base;

namespace Social_Media.Repository.UserRepository
{
    public interface IUserRepository : IBaseRepository<User, int>
    {
        Task<PageResult<PostFeedDTO>> GetUserPostFeedAsync(SearchPostFeedRequest searchPostFeedRequest);
        Task<PageResult<PostFeedTableDTO>> GetAllPostsTable(
                 SearchPostsAdminDTO searchPostFeedRequest);

        Task<PageResult<PostReactorDTO>> GetPostReactions(
           int postId, ReactionType? reactionType,
           SearchPostFeedRequest searchPostFeedRequest);

        Task<ReactionResult> ToggleReactionAsync(int postId, ReactionType reactionType);
 
    }
}
