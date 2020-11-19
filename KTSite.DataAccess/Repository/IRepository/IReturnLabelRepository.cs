using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IReturnLabelRepository : IRepository<ReturnLabel>
    {
        public IEnumerable<Order> getAllOrdersOfUser(string userNameId);
        void update(ReturnLabel returnLabel);
        ReturnLabel Get(long id);
    }
}
