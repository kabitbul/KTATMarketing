using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;

namespace KTSite.DataAccess.Repository
{
    public class PaymentHistoryMerchRepository : Repository<PaymentHistoryMerch> , IPaymentHistoryMerchRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentHistoryMerchRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
