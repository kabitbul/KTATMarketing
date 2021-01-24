using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System.Linq;

namespace KTSite.DataAccess.Repository
{
    public class PaymentBalanceBackupRepository : Repository<PaymentBalanceBackup> , IPaymentBalanceBackupRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentBalanceBackupRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(PaymentBalanceBackup paymentBalanceBackup)
        {
            var objFromDb = _db.PaymentBalanceBackups.FirstOrDefault(s=>s.Id == paymentBalanceBackup.Id);
            if (objFromDb != null)
            {
                //objFromDb.UserNameId = paymentBalance.UserNameId;
                //objFromDb.Balance = paymentBalance.Balance;
            }
        }
    }
}
