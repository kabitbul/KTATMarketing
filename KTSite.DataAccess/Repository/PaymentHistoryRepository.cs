using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class PaymentHistoryRepository : Repository<PaymentHistory> , IPaymentHistoryRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentHistoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<PaymentHistory> getHistoryOfAdminPayment()
        {
            IEnumerable<PaymentHistory> payHistoryList = 
                _db.PaymentHistories.FromSqlRaw("select * from PaymentHistories where SentFromAddressId IN (select p.Id from PaymentSentAddresses p where" +
                " p.IsAdmin = 1) order by PayDate desc");
            return payHistoryList;
        }
        public IEnumerable<PaymentHistory> getHistoryOfAllUsersPayment()
        {
            IEnumerable<PaymentHistory> payHistoryList =
                _db.PaymentHistories.FromSqlRaw("select * from PaymentHistories where SentFromAddressId IN (select p.Id from PaymentSentAddresses p where" +
                " p.IsAdmin = false) order by PayDate desc");
                
                //_db.PaymentHistories.FromSqlRaw("select * from PaymentHistories");
            return payHistoryList;
        }
        public void update(PaymentHistory paymentHistory)
        {
            var objFromDb = _db.PaymentHistories.FirstOrDefault(s=>s.Id == paymentHistory.Id);
            if (objFromDb != null)
            {
                objFromDb.UserNameId = paymentHistory.UserNameId;
                objFromDb.Amount = paymentHistory.Amount;
                objFromDb.Status = paymentHistory.Status;
                objFromDb.PayDate = paymentHistory.PayDate;
                objFromDb.SentFromAddressId = paymentHistory.SentFromAddressId;
            }
        }
    }
}
