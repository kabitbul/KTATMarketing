using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentBalanceMerchRepository :IRepository<PaymentBalanceMerch>
    {
        void update(PaymentBalanceMerch paymentBalanceMerch);
        PaymentBalanceMerch Get(long id);
    }
}
