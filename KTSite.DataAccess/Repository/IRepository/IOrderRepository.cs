using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IOrderRepository :IRepository<Order>
    {
        void update(Order order);
        Order Get(long id);
    }
}
