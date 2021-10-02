using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System.Linq;

namespace KTSite.DataAccess.Repository
{
    public class PaymentMethodMerchRepository : Repository<PaymentMethodMerch> , IPaymentMethodMerchRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentMethodMerchRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(PaymentMethodMerch paymentMethodMerch)
        {
            var objFromDb = _db.PaymentMethodMerchs.FirstOrDefault(s=>s.Id == paymentMethodMerch.Id);
            if (objFromDb != null)
            {
                objFromDb.PaymentType = paymentMethodMerch.PaymentType;
                objFromDb.PaymentTypeAddress = paymentMethodMerch.PaymentTypeAddress;
                objFromDb.PrefferdMethod = paymentMethodMerch.PrefferdMethod;
            }
        }
    }
}
