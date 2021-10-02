using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;

namespace KTSite.DataAccess.Repository
{
    public class PaymentHistoryArchiveRepository : Repository<PaymentHistoryArchive> , IPaymentHistoryArchiveRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentHistoryArchiveRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
