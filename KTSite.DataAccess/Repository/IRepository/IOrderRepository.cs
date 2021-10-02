using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IOrderRepository :IRepository<Order>
    {
        void update(Order order);
        Order Get(long id);
    }
}
