using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IOrderArchiveRepository :IRepository<OrderArchive>
    {
       
        OrderArchive Get(long id);
    }
}
