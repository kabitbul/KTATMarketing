using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentHistoryRepository :IRepository<PaymentHistory>
    {
        public IEnumerable<PaymentHistory> getHistoryOfAdminPayment();
        public IEnumerable<PaymentHistory> getHistoryOfAllUsersPayment();
        void update(PaymentHistory paymentHistory);
        PaymentHistory Get(long id);
    }
}
