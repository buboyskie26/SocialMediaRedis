using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_Media.Data;
using Social_Media.DTO;
using Social_Media.DTO.Post;
using Social_Media.DTO.Reaction;
using Social_Media.Enums;
using Social_Media.Repository.UserRepository;

namespace Social_Media.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly SocialMediaDBContext _context;

        public UserController(IUserRepository userRepository, SocialMediaDBContext context  )
        {
            _userRepository = userRepository;
            _context = context;
        }

        [HttpGet("check-connection")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                // 1. Verify if we can communicate with the database
                var canConnect = await _context.Database.CanConnectAsync();

                if (!canConnect)
                {
                    return StatusCode(500, new { Success = false, Message = "Could not establish connection to Supabase." });
                }

                // 2. Fetch the number of users currently in the Supabase database
                var userCount = await _context.Users.CountAsync();

                // 3. Fetch top 5 usernames to verify we can read actual data
                var previewUsers = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new { u.Id, u.Username, u.Email, u.CreatedAt })
                    .ToListAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Successfully connected to Supabase PostgreSQL!",
                    DatabaseSchema = "dasd",
                    TotalUsersInDb = userCount,
                    RecentUsersPreview = previewUsers
                });
            }
            catch (Exception ex)
            {
                // If there's an error, this will output the exact database error message
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while connecting to the database.",
                    Details = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet("get-feed")]
        public async Task<IActionResult> GetUserFeed(
            [FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            var searchPostFeedRequest = new SearchPostFeedRequest
            {
                PageNumber = pageNumber < 1 ? 1 : pageNumber,
                PageSize = pageSize < 1 || pageSize > 100 ? 10 : pageSize // clamp to a sane max
            };
            var result = await _userRepository.GetUserPostFeedAsync(searchPostFeedRequest);
            return Ok(result);
        }

        [HttpPost("Get-all-posts")]
        public async Task<IActionResult> GetUserFeed(
            [FromBody] SearchPostsAdminDTO searchPostFeedRequest)
            //[FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            var result = await _userRepository.GetAllPostsTable(searchPostFeedRequest);
            return Ok(result);
        }

        // /api/posts/{postId}/reactions

        // GET /api/posts/{postId}/reactions?reactionType=Like&pageNumber=1&pageSize=20
        [HttpGet("{postId:int}/reactions")]
        public async Task<IActionResult> GetPostReactions(
            //[FromBody] PostReactionParams postReactionParams,
            int postId,
            [FromQuery] ReactionType? reactionType,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var searchPostFeedRequest = new SearchPostFeedRequest
            {
                PageNumber = pageNumber < 1 ? 1 : pageNumber,
                PageSize = pageSize < 1 || pageSize > 100 ? 10 : pageSize // clamp to a sane max
            };

            var getPostReaction = await _userRepository.GetPostReactions(
                postId, reactionType, searchPostFeedRequest);

            return Ok(getPostReaction);
        }

        [HttpPost("toggleReaction/{postId}")]
        public async Task<IActionResult> ToggleReaction(int postId, [FromBody] ReactionRequest reactionRequest)
        {
            var toggleReactionResult = await _userRepository.ToggleReactionAsync(postId, reactionRequest.ReactionType);

            return toggleReactionResult switch
            {
                ReactionResult.Added => Ok(new { message = "Reaction added", status = "added" }),
                ReactionResult.Removed => Ok(new { message = "Reaction removed", status = "removed" }),
                ReactionResult.Updated => Ok(new { message = "Reaction updated", status = "updated" }),
                _ => BadRequest()
            };
        }
    }
}
