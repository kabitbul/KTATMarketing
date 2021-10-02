using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentHistoryMerchRepository :IRepository<PaymentHistoryMerch>
    {
        PaymentHistoryMerch Get(long id);
    }
}
