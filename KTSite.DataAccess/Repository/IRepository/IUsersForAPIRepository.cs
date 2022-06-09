using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IUsersForAPIRepository : IRepository<UsersForAPI>
    {
        void update(UsersForAPI usersForAPI);
    }
}
