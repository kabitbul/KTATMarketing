using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System.Linq;

namespace KTSite.DataAccess.Repository
{
    public class PaymentBalanceMerchRepository : Repository<PaymentBalanceMerch> , IPaymentBalanceMerchRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentBalanceMerchRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(PaymentBalanceMerch paymentBalanceMerch)
        {
            var objFromDb = _db.PaymentBalancesMerch.FirstOrDefault(s=>s.Id == paymentBalanceMerch.Id);
            if (objFromDb != null)
            {
                objFromDb.UserNameId = paymentBalanceMerch.UserNameId;
                objFromDb.Balance = paymentBalanceMerch.Balance;
            }
        }
    }
}
