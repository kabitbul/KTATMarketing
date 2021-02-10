using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System.Linq;

namespace KTSite.DataAccess.Repository
{
    public class RefundRepository : Repository<Refund> , IRefundRepository
    {
        private readonly ApplicationDbContext _db;
        public RefundRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Refund refund)
        {
            var objFromDb = _db.Refunds.FirstOrDefault(s=>s.Id == refund.Id);
            if (objFromDb != null)
            {
                objFromDb.OrderId = refund.OrderId;
                objFromDb.RefundQuantity = refund.RefundQuantity;
                objFromDb.RefundedBy = refund.RefundedBy;
                objFromDb.RefundDate = refund.RefundDate;
                objFromDb.Cost = refund.Cost;
                objFromDb.Quantity = refund.Quantity;
                objFromDb.UserNameId = refund.UserNameId;
                objFromDb.StoreNameId = refund.StoreNameId;
            }
        }
    }
}
