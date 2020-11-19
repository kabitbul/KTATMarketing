using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class PaymentSentAddressRepository : Repository<PaymentSentAddress> , IPaymentSentAddressRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentSentAddressRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(PaymentSentAddress paymentSentAddress)
        {
            var objFromDb = _db.PaymentSentAddresses.FirstOrDefault(s=>s.Id == paymentSentAddress.Id);
            if (objFromDb != null)
            {
                objFromDb.UserNameId = paymentSentAddress.UserNameId;
                objFromDb.PaymentTypeAddress = paymentSentAddress.PaymentTypeAddress;
                objFromDb.PaymentType = paymentSentAddress.PaymentType;
                objFromDb.IsAdmin = paymentSentAddress.IsAdmin;
            }
        }
    }
}
