using Azure;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Social_Media.Data;
using Social_Media.DTO;
using Social_Media.DTO.Pagination;
using Social_Media.DTO.Post;
using Social_Media.DTO.Reaction;
using Social_Media.DTO.User;
using Social_Media.Entities;
using Social_Media.Enums;
using Social_Media.Exceptions;
using Social_Media.Repository.Base;
using Social_Media.Repository.FriendRepository;
using Social_Media.Service.CurrentService;
using System;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Social_Media.Repository.UserRepository
{
    public class UserRepository : BaseRepository<User, int>, IUserRepository
    {
        private readonly SocialMediaDBContext _socialMediaDBContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFriendRepository _friendRepository;

        public UserRepository(SocialMediaDBContext socialMediaDBContext,
            ICurrentUserService currentUserService,
            IFriendRepository friendRepository) : base(socialMediaDBContext)
        {
            _socialMediaDBContext = socialMediaDBContext;
            _currentUserService = currentUserService;
            _friendRepository = friendRepository;
        }
        public async Task<IEnumerable<PostFeedDTO>> GetUserPostFeedAsyncv2(
            int pageNumber = 1, int pageSize = 10)
        {

            var userLoggedInId = 1;

            var friendAcceptedStatus = FriendshipStatus.Accepted;
            var publicPost = PostVisibility.Public;

            // Get User Friends Ids

            var userFriendIds = _socialMediaDBContext.Friendships
                .AsNoTracking()
                .Where(w => w.Status == friendAcceptedStatus && 
                    (w.RequesterId == userLoggedInId || w.AddresseeId == userLoggedInId))

                .Select(e => e.RequesterId == userLoggedInId ? e.AddresseeId : e.RequesterId);

            var posts = await _socialMediaDBContext.Posts

                .AsNoTracking()

                .Where(w => w.AuthorId == userLoggedInId 
                    || (userFriendIds.Contains(w.AuthorId) && w.Visibility == publicPost))

                .Select(e => new PostFeedDTO
                {
                    AuthorId = e.AuthorId,
                    Content = e.Content,
                    AuthorUsername = e.Author.Username
                })
                .ToListAsync();

            return posts;
        }

        //
        //public async Task<IEnumerable<PostFeedDTO>> GetUserPostFeedAsync(
        //    SearchPostFeedRequest searchPostFeedRequest)
        //{

        public async Task<PageResult<PostFeedDTO>> GetUserPostFeedAsync(
            SearchPostFeedRequest searchPostFeedRequest)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == 0)
                throw new UnauthorizedAccessExceptionClass("This route requires authentication. Please login first.");

            var userLoggedInId = _currentUserService.UserId;

            var publicPost = PostVisibility.Public;

            // Deferred execution. No Database roundtrip.
            // Builds an expression tree representing the SQL query,
            // (Does'nt execute in SQL Server)
            var userFriendsId = await _friendRepository.GetFriendIdsAsync(userLoggedInId);

            // Base query
            var allPosts = _socialMediaDBContext.Posts
                .AsNoTracking()
                .Where(w => w.AuthorId == userLoggedInId
                    || (w.Visibility == publicPost && userFriendsId.Contains(w.AuthorId)));

            var totalCount = await allPosts.CountAsync();

            // Fetch both My Posts and Friends' Public Posts in a SINGLE query
            var feedPosts = await allPosts
                .OrderByDescending(w => w.CreatedAt)
                // Pagination
                .Skip((searchPostFeedRequest.PageNumber - 1) * searchPostFeedRequest.PageSize)
                .Take(searchPostFeedRequest.PageSize)
                .Select(p => new PostFeedDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    AuthorUsername = p.Author.Username, // EF Core handles joins automatically (no need for include Author)
                    CreatedAt = p.CreatedAt,
                    CommentsCount = p.CommentsCount,   // direct column read, no aggregation
                    ReactionsCount = p.ReactionsCount   // direct column read, no aggregation
                })
                .ToListAsync();

            // Exit early if no posts are returned
            if (feedPosts.Any() == false)
                return new PageResult<PostFeedDTO>
                {
                    Items = feedPosts,
                    TotalCount = totalCount
                };
            //
            return new PageResult<PostFeedDTO>
            {
                Items = feedPosts,
                TotalCount = totalCount,
                PageNumber = searchPostFeedRequest.PageNumber,
                PageSize = searchPostFeedRequest.PageSize
            };
        }
        //
        public async Task<PageResult<PostFeedDTO>> GetUserPostFeedAsyncOrig(
            SearchPostFeedRequest searchPostFeedRequest)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == 0)
                throw new UnauthorizedAccessExceptionClass("This route requires authentication. Please login first.");

            var userLoggedInId = _currentUserService.UserId;

            var publicPost = PostVisibility.Public;
 
            var userFriendsId = GetFriendIds(userLoggedInId);

            // Base query
            var allPosts =  _socialMediaDBContext.Posts
                .AsNoTracking()
                 .Where(p => !p.IsDeleted)  
                 .Where(w => w.AuthorId == userLoggedInId
                    || (w.Visibility == publicPost && userFriendsId.Contains(w.AuthorId)));

            var totalCount = await allPosts.CountAsync();

            // Fetch both My Posts and Friends' Public Posts in a SINGLE query
            var feedPosts = await allPosts
                .OrderByDescending(w=> w.CreatedAt)
                // Pagination
                .Skip((searchPostFeedRequest.PageNumber - 1) * searchPostFeedRequest.PageSize)
                .Take(searchPostFeedRequest.PageSize)
                .Select(p => new PostFeedDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    AuthorUsername = p.Author.Username, // EF Core handles joins automatically (no need for include Author)
                    CreatedAt = p.CreatedAt,
                    CommentsCount = p.CommentsCount,   // direct column read, no aggregation
                    ReactionsCount = p.ReactionsCount   // direct column read, no aggregation
                })
                .ToListAsync();

            // Exit early if no posts are returned
            if (feedPosts.Any() == false)
                return new PageResult<PostFeedDTO>
                {
                    Items = feedPosts,
                    TotalCount = totalCount
                };

            var feedPostsIds = feedPosts.Select(w => w.Id).ToList();

            var flatReactions = await _socialMediaDBContext.PostReactions
                .AsNoTracking()
                .Where(w => feedPostsIds.Contains(w.PostId))
                .Select(w => new    //  projection instead of Include
                {
                    //w.UserId,
                    w.PostId,
                    //w.User.Username,
                    w.ReactionType
                }).ToListAsync();  // one clean, narrow SQL round-trip

            // Dictionary lookup O(1)
            var reactionDict2 = flatReactions
                .GroupBy(w => w.PostId)   // grouping now explicitly client-side, no ambiguity
                .ToDictionary(w => w.Key, w => w.Select(r => new PostReactionDTO
                {
                    //PostReactorId = r.UserId,
                    //PostReactorUserName = r.Username ?? "",
                    ReactionTypeEnum = r.ReactionType
                }).ToList());
 
            // Bad because of .Include
            //var reactionDict = await _socialMediaDBContext.PostReactions
            //    .AsNoTracking()
            //    .Include(w=> w.User)
            //    .Where(w => feedPostsIds.Contains(w.PostId))

            //    .GroupBy(w => w.PostId)
            //    .ToDictionaryAsync(g => g.Key, g => g.Select(r => new PostReactionDTO
            //    {
            //        PostReactorId = r.UserId,
            //        PostReactorUserName = r.User.Username ?? "",
            //        ReactionTypeEnum = r.ReactionType
            //    }).ToList());
 
            // The Optimized "Batch-Querying & Stitching" Pattern. https://eu.chat.pwc.com/c/2417c0ce-4aac-400e-9fca-8fc1b48995c4
            // This returns at most 10 rows (one for each post ID), containing only two integers each.

            //var commentCountDict = await _socialMediaDBContext.Comments
            //    .AsNoTracking()
            //    .Where(w => feedPostsIds.Contains(w.PostId))
            //    .GroupBy(w => w.PostId)
            //    .ToDictionaryAsync(g => g.Key, g => g.Count());

            foreach (var item in feedPosts)
            {
                if (reactionDict2.TryGetValue(item.Id, out var postReactionDTO))
                {
                    var lookUp = postReactionDTO.ToLookup(r => r.ReactionTypeEnum);

                    //item.ReactionsByType = Enum.GetValues<ReactionType>()
                    //    .ToDictionary(w => w, w => lookUp[w].ToList());

                    //item.ReactionsByTypeCount = Enum.GetValues<ReactionType>()
                    //    .ToDictionary(w => w, w => lookUp[w].Count());
                    // 
                }
            }
            
            //
            return new PageResult<PostFeedDTO>
            {
                Items = feedPosts,
                TotalCount = totalCount,
                PageNumber = searchPostFeedRequest.PageNumber,
                PageSize = searchPostFeedRequest.PageSize
            };
        }

        // User Clicked the Reactions Div.
        // GET /api/posts/{postId}/reactions?reactionType=Like&pageNumber=1&pageSize=20
        public async Task<PageResult<PostReactorDTO>> GetPostReactions(
            int postId, ReactionType? reactionType,
            SearchPostFeedRequest searchPostFeedRequest)
        {
            // deffered query
            var query = _socialMediaDBContext.PostReactions
                .AsNoTracking()
                 .Where(w => w.PostId == postId);

            if (reactionType.HasValue)
                query = query.Where(w => w.ReactionType == reactionType.Value);

            var totalCount = await query.CountAsync();

            if (totalCount == 0)
                return new PageResult<PostReactorDTO>()
                {
                    Items = [],
                    TotalCount = 0,
                    PageNumber = searchPostFeedRequest.PageNumber,
                    PageSize = searchPostFeedRequest.PageSize
                };

            // Indexing:
            // CREATE NONCLUSTERED INDEX IX_PostReactions_PostId_ReactionType_CreatedAt
            // ON PostReactions(PostId, ReactionType, CreatedAt DESC)
            // INCLUDE(UserId);

            //

            // Your projection also pulls Username/DisplayName from Users

            // CREATE NONCLUSTERED INDEX IX_Users_Id_Username_DisplayName
            // ON Users(Id) INCLUDE(Username, DisplayName);
            //
            var getPostReactions = await query
                 .OrderByDescending(w => w.CreatedAt)
                 .Skip((searchPostFeedRequest.PageNumber - 1) * searchPostFeedRequest.PageSize)
                 .Take(searchPostFeedRequest.PageSize)
                 .Select(w => new PostReactorDTO           // projection -> single SQL JOIN, no N+1
                 {
                     UserId = w.UserId,
                     Username = w.User.Username,                     DisplayName = w.User.DisplayName,
                     ReactionTypeEnum = w.ReactionType
                 })
                 .ToListAsync();

            return new PageResult<PostReactorDTO>
            {
                Items = getPostReactions,
                TotalCount = totalCount,
                PageNumber = searchPostFeedRequest.PageNumber,
                PageSize=searchPostFeedRequest.PageSize
            };
        }
      
        
        // Todo, Add reaction (race condition.)

        public async Task<ReactionResult> ToggleReactionAsyncv2(int postId, ReactionType reactionType)
        {
            var userId = 1;

            var existingReaction = await _socialMediaDBContext.PostReactions
                .FirstOrDefaultAsync(w => w.PostId == postId && w.UserId == userId);
                
            if(existingReaction is null)
            {
                // Add Reaction
                _socialMediaDBContext.PostReactions.Add(new PostReaction
                {
                    PostId = postId,
                    UserId = userId,
                    ReactionType = reactionType,
                    CreatedAt = DateTime.UtcNow
                });

                // Increment the PostReaction Count
                await _socialMediaDBContext.Posts
                    .Where(w => w.Id == postId)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.ReactionsCount, p => p.ReactionsCount + 1));

                await _socialMediaDBContext.SaveChangesAsync();

                return ReactionResult.Added;
            }

            // Existing side..

            // Removed
            if (existingReaction.ReactionType == reactionType)
            {
                _socialMediaDBContext.PostReactions.Remove(existingReaction);
                // 
                await _socialMediaDBContext.Posts
                    .Where(w => w.Id == postId)
                    .ExecuteUpdateAsync(w => w.SetProperty(e => e.ReactionsCount, e => e.ReactionsCount - 1));

                await _socialMediaDBContext.SaveChangesAsync();

                return ReactionResult.Removed;
            }

            // Same user, different reaction type — count doesn't change

            existingReaction.ReactionType = reactionType;
            await _socialMediaDBContext.SaveChangesAsync();
            return ReactionResult.Updated;

        }

        public async Task<ReactionResult> ToggleReactionAsync(int postId, ReactionType reactionType)
        {
            var userId = 1; // Replace with your real user ID retriever

            using var transaction = await _socialMediaDBContext.Database.BeginTransactionAsync();

            try
            {
                var existingReaction = await _socialMediaDBContext.PostReactions
                    .FirstOrDefaultAsync(w => w.PostId == postId && w.UserId == userId);

                if (existingReaction is null)
                {
                    // Add Reaction Row
                    _socialMediaDBContext.PostReactions.Add(new PostReaction
                    {
                        PostId = postId,
                        UserId = userId,
                        ReactionType = reactionType,
                        CreatedAt = DateTime.UtcNow
                    });

                    // Increment the PostReaction Count
                    await _socialMediaDBContext.Posts
                        .Where(w => w.Id == postId)
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.ReactionsCount, p => p.ReactionsCount + 1));

                    // Save changes. If a concurrent thread already inserted this reaction,
                    // this will throw a DbUpdateException because of our Unique Index constraint.
                    await _socialMediaDBContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return ReactionResult.Added;
                }

                // If removing the reaction
                if (existingReaction.ReactionType == reactionType)
                {
                    _socialMediaDBContext.PostReactions.Remove(existingReaction);

                    await _socialMediaDBContext.Posts
                        .Where(w => w.Id == postId)
                        .ExecuteUpdateAsync(w => w.SetProperty(e => e.ReactionsCount, e => e.ReactionsCount - 1));

                    await _socialMediaDBContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return ReactionResult.Removed;
                }

                // If updating the reaction type (No change to total count)
                existingReaction.ReactionType = reactionType;
                await _socialMediaDBContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return ReactionResult.Updated;
            }
            catch (DbUpdateException)
            {
                // If a unique constraint violation happens, rollback the transaction.
                // This reverses the ExecuteUpdateAsync (+1) so your count stays accurate.
                await transaction.RollbackAsync();

                // Since the insert failed because it already exists, we fall back to either:
                // 1. Throwing a custom exception
                // 2. Or returning a specific status, e.g., treating it as no-op or trying to update it.
                throw new InvalidOperationException("This reaction has already been processed.");
            }
        }


        public async Task<Comment> CreateCommentAsync(int postId, int authorId, string content)
        {

            var comment = new Comment
            {
                PostId = postId,
                AuthorId = authorId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _socialMediaDBContext.Comments.Add(comment);

            // Atomic increment - single UPDATE statement, no race condition
            await _socialMediaDBContext.Posts
                .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.CommentsCount, p => p.CommentsCount + 1)
            );

            await _context.SaveChangesAsync();
            return comment;
        }
        //public async Task<HashSet<int>> GetFriendIdsAsync(int userLoggedInId)
        //{
        //    var userAddresseeFriendsId = _socialMediaDBContext.Friendships
        //        .AsNoTracking()
        //        .Where(w => w.RequesterId == userLoggedInId && w.Status == FriendshipStatus.Accepted)
        //        .Select(w => w.AddresseeId);

        //    var userRequesterFriendsId = _socialMediaDBContext.Friendships
        //        .AsNoTracking()
        //        .Where(w => w.AddresseeId == userLoggedInId && w.Status == FriendshipStatus.Accepted)
        //        .Select(w => w.RequesterId);

        //    var unionFriends = await userAddresseeFriendsId.Union(userRequesterFriendsId)
        //        .ToListAsync();

        //    return unionFriends.ToHashSet();
        //}

        public IQueryable<int> GetFriendIds(int userLoggedInId)
        {
            var userAddresseeFriendsId = _socialMediaDBContext.Friendships
                .AsNoTracking()
                .Where(w => w.RequesterId == userLoggedInId && w.Status == FriendshipStatus.Accepted)
                .Select(w => w.AddresseeId);

            var userRequesterFriendsId = _socialMediaDBContext.Friendships
                .AsNoTracking()
                .Where(w => w.AddresseeId == userLoggedInId && w.Status == FriendshipStatus.Accepted)
                .Select(w => w.RequesterId);

            //var unionFriends = new HashSet<int>(userAddresseeFriendsId.Union(userRequesterFriendsId));

            //return unionFriends.AsQueryable();

            return userAddresseeFriendsId.Union(userRequesterFriendsId);
        }

        // Admin Side.
        public async Task<PageResult<PostFeedTableDTO>> GetAllPostsTable(
            SearchPostsAdminDTO searchPostFeedRequest)
        {
            // deferred.
            IQueryable<Post> getAllPosts = _socialMediaDBContext.Posts
                .AsNoTracking();

            // MSSQL Full text search.
            if (!string.IsNullOrWhiteSpace(searchPostFeedRequest.SearchKeyword))
            {
                // Avoid .ToLower() since SQL Server is case-insensitive by default.
                var keyword = searchPostFeedRequest.SearchKeyword
                    .Trim();
                // Better to use Full text search..
                getAllPosts = getAllPosts.Where(w => EF.Functions.Like(w.Content, $"%{keyword}%"));
            }

            // Filter by Authors Id.

            if (searchPostFeedRequest.AuthorIds is { Count: > 0 })
            {
                //var distinctAuthorIds = searchPostFeedRequest.AuthorIds.Distinct().ToList();

                var distinctAuthorIds = new HashSet<int>(searchPostFeedRequest.AuthorIds);

                getAllPosts = getAllPosts.Where(post => distinctAuthorIds.Contains(post.AuthorId));
            }

            // Filter by Post Visibility.
            if (searchPostFeedRequest.PostVisibilityTypes is { Count: > 0 })
            {
                //var visibilityTypes = searchPostFeedRequest.PostVisibilityTypes.Distinct().ToList();
                var visibilityTypes = new HashSet<PostVisibility>(searchPostFeedRequest.PostVisibilityTypes);

                getAllPosts = getAllPosts.Where(post => visibilityTypes.Contains(post.Visibility));
            }

            var postsCount = await getAllPosts.CountAsync();

            if (postsCount == 0)
                return new PageResult<PostFeedTableDTO>
                {
                    Items = [],
                    TotalCount = 0,
                    PageNumber = searchPostFeedRequest.PageNumber,
                    PageSize = searchPostFeedRequest.PageSize
                };
            //
            var posts = await getAllPosts
                .OrderByDescending(w => w.CreatedAt)
                .Skip((searchPostFeedRequest.PageNumber - 1) * searchPostFeedRequest.PageSize)
                .Take(searchPostFeedRequest.PageSize)
                .Select(w => new PostFeedTableDTO
                {
                    Id = w.Id,
                    Content = w.Content,
                    AuthorId = w.AuthorId,
                    AuthorUsername = w.Author.Username, // Join to User Table
                    CreatedAt = w.CreatedAt,
                    Visibility = w.Visibility,
                    // Left join promotes Cartesian product.
                    PostReactionDTO = w.PostReactions.Select(e => new PostReactionDTO
                    {
                        PostReactorId = e.UserId,
                        PostReactorUserName = e.User.Username,
                        ReactionTypeEnum = e.ReactionType
                    }).ToList()
                })
                .ToListAsync();

            var postIds = posts.Select(w => w.Id);

            // In memory stiching and Batch Querying.

            //CREATE NONCLUSTERED INDEX IX_PostReactions_PostdD
            // ON PostReactions(PostId)
            // INCLUDE(UserId, ReactionType).

            // Without an index, if your database has 10 million reactions,
            // SQL Server will scan all 10 million rows to find the reactions for
            // your 10 - 20 page posts.

            var postReactions = await _socialMediaDBContext.PostReactions
                .AsNoTracking()
                .Where(w => postIds.Contains(w.PostId))
                .Select(w => new
                {
                    w.PostId,
                    w.UserId,
                    w.ReactionType,
                    ReactionUserName = w.User.Username,
                })
                .ToListAsync();


            var postReactionDict = postReactions
                .GroupBy(w => w.PostId)
                .ToDictionary(w => w.Key, w => w.Select(e => new PostReactionDTO()
                {
                    PostReactorId = e.UserId,
                    PostReactorUserName= e.ReactionUserName,
                    ReactionTypeEnum = e.ReactionType
                }).ToList());
                

            foreach (var item in posts)
            {
                if (postReactionDict.TryGetValue(item.Id, out var getPostReaction))
                {
                    item.PostReactionDTO = getPostReaction;
                }
            }

            return new PageResult<PostFeedTableDTO>()
            {
                Items = posts,
                TotalCount = postsCount,
                PageNumber = searchPostFeedRequest.PageNumber,
                PageSize = searchPostFeedRequest.PageSize
            };
        }
    }
}