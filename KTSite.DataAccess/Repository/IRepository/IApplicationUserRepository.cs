using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository :IRepository<ApplicationUser>
    {
        public IEnumerable<ApplicationUser> GeAllUsersWithoutrecInPayBalance();
            void update(ApplicationUser applicationUser);        
    }
}
