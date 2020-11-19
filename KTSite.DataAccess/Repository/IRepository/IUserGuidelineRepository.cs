using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IUserGuidelineRepository : IRepository<UserGuideline>
    {
        //public IEnumerable<Order> getAllOrdersOfUser(string userNameId);
        void update(UserGuideline userGuideline);
        //ReturnLabel Get(long id);
    }
}
