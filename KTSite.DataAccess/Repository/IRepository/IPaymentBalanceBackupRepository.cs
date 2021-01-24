using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentBalanceBackupRepository :IRepository<PaymentBalanceBackup>
    {
        void update(PaymentBalanceBackup paymentBalanceBackup);
        PaymentBalanceBackup Get(long id);
    }
}
