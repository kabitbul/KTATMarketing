using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentMethodMerchRepository :IRepository<PaymentMethodMerch>
    {
        void update(PaymentMethodMerch paymentMethodMerch);
        PaymentMethodMerch Get(long id);
    }
}
