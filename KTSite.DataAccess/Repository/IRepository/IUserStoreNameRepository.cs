using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IUserStoreNameRepository : IRepository<UserStoreName>
    {
        void update(UserStoreName userStoreName);
    }
}
