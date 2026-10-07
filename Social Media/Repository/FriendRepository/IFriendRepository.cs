namespace Social_Media.Repository.FriendRepository
{
    public interface IFriendRepository
    {
        Task<HashSet<int>> GetFriendIdsAsync(int userLoggedInId);
    }
}
