using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class PaymentBalanceRepository : Repository<PaymentBalance> , IPaymentBalanceRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentBalanceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(PaymentBalance paymentBalance)
        {
            var objFromDb = _db.PaymentBalances.FirstOrDefault(s=>s.Id == paymentBalance.Id);
            if (objFromDb != null)
            {
                objFromDb.UserNameId = paymentBalance.UserNameId;
                objFromDb.Balance = paymentBalance.Balance;
            }
        }
    }
}
