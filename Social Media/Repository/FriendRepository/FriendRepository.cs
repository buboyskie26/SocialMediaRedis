using Microsoft.EntityFrameworkCore;
using Social_Media.Data;
using Social_Media.Entities;
using Social_Media.Enums;
using StackExchange.Redis;

namespace Social_Media.Repository.FriendRepository
{
    public class FriendRepository : IFriendRepository
    {
        private readonly SocialMediaDBContext _socialMediaDBContext;
        //private readonly IConnectionMultiplexer _redis;
        //private readonly IDatabase _cache;


        // TTL = Time to Live
        private static readonly TimeSpan FriendsCacheTtl = TimeSpan.FromHours(24);

        //public FriendRepository(SocialMediaDBContext socialMediaDBContext,
        //        IConnectionMultiplexer redis)
        //{
        //    _socialMediaDBContext = socialMediaDBContext;
        //    _redis = redis;
        //    _cache = redis.GetDatabase();
        //}
        public FriendRepository(SocialMediaDBContext socialMediaDBContext)
        {
            _socialMediaDBContext = socialMediaDBContext;
        }

        public async Task<HashSet<int>> GetFriendIdsAsync(int userLoggedInId)
        {
            //
            //var cacheKey = $"users:{userLoggedInId}:friends";

            //var isCacheExist = await _cache.KeyExistsAsync(cacheKey);
            //// 1. Check if the key exists in Redis
            //if (isCacheExist)
            //{
            //    // Fetch all members of the Redis Set
            //    var cachedMembers = await _cache.SetMembersAsync(cacheKey);

            //    // Filter out any sentinel marker used for caching empty states
            //    return cachedMembers
            //        .Select(m => (int)m)
            //        .Where(id => id != -1) // -1 is our empty-state marker (explained below)
            //        .ToHashSet();
            //}

            //// 2. Cache Miss: Fetch from SQL Database
            //var friendIdsFromDb = await GetFriendIdsFromDbAsync(userLoggedInId);

            //// 3. Populate redis
            //if(friendIdsFromDb.Count > 0)
            //{
            //    var redisValues = friendIdsFromDb.Select(id => (RedisValue)id).ToArray();
                
            //    // SADD adds all items to the Set in a single round-trip
            //    await _cache.SetAddAsync(cacheKey, redisValues);
            //}
            //else
            //{
            //    // Prevent "Cache Penetration" for users with 0 friends:
            //    // Store a dummy/sentinel value (-1) so Redis knows the user has 0 friends.

            //    await _cache.SetAddAsync(cacheKey, -1);
            //}

            //// Set an expiration so stale data eventually refreshes naturally

            //await _cache.KeyExpireAsync(cacheKey, FriendsCacheTtl);

            //return friendIdsFromDb;
            // If we have no Redis
            return await GetFriendIdsFromDbAsync(userLoggedInId);    
        }
        private async Task<HashSet<int>> GetFriendIdsFromDbAsync(int userLoggedInId)
        {

            var userAddresseeFriendsId = _socialMediaDBContext.Friendships
               .AsNoTracking()
               .Where(w => w.RequesterId == userLoggedInId && w.Status == FriendshipStatus.Accepted)
               .Select(w => w.AddresseeId);

            var userRequesterFriendsId = _socialMediaDBContext.Friendships
                .AsNoTracking()
                .Where(w => w.AddresseeId == userLoggedInId && w.Status == FriendshipStatus.Accepted)
                .Select(w => w.RequesterId);

            var allFriendsId = await userAddresseeFriendsId.Union(userRequesterFriendsId).ToListAsync();

            return allFriendsId.ToHashSet();
        }
    }
}
