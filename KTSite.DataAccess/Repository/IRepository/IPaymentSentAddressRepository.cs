using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IPaymentSentAddressRepository :IRepository<PaymentSentAddress>
    {
        void update(PaymentSentAddress paymentSentAddress);
        PaymentSentAddress Get(int id);
    }
}
