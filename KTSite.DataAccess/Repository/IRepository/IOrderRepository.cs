using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IOrderRepository :IRepository<Order>
    {
        void update(Order order);
        Order Get(long id);
        public IEnumerable<Order> GetAllOrders();
    }
}
