using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IComplaintsRepository :IRepository<Complaints>
    {
        public IEnumerable<Order> getAllOrdersOfUser(string userNameId);
        public IEnumerable<Order> getAllOrdersForAdmin();
        void update(Complaints complaints);
        Complaints Get(long id);
    }
}
