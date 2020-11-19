using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentBalanceRepository :IRepository<PaymentBalance>
    {
        void update(PaymentBalance paymentBalance);
        PaymentBalance Get(long id);
    }
}
