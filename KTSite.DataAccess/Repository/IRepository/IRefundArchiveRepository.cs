using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IRefundArchiveRepository :IRepository<RefundArchive>
    {
        RefundArchive Get(long id);
    }
}
