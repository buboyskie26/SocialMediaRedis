using Social_Media.Data;
using Social_Media.Entities;
using Social_Media.Repository.Base;

namespace Social_Media.Repository.UserAccessRepository
{
    public class UserAccessRepository : BaseRepository<UserAccess, int>, IUserAccessRepository
    {
        private readonly SocialMediaDBContext _socialMediaDBContext;
        public UserAccessRepository(SocialMediaDBContext socialMediaDBContext) : base(socialMediaDBContext)
        {
            _socialMediaDBContext = socialMediaDBContext;
        }
    }
}
