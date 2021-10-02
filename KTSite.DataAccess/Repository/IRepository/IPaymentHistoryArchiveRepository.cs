using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentHistoryArchiveRepository :IRepository<PaymentHistoryArchive>
    {
        PaymentHistoryArchive Get(long id);
    }
}
