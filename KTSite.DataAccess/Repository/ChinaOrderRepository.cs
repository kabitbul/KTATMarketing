using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class ChinaOrderRepository : Repository<ChinaOrder> , IChinaOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public ChinaOrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(ChinaOrder chinaOrder)
        {
            var objFromDb = _db.ChinaOrders.FirstOrDefault(s=>s.Id == chinaOrder.Id);
            if (objFromDb != null)
            {
                objFromDb.ProductId = chinaOrder.ProductId;
                objFromDb.Quantity = chinaOrder.Quantity;
                objFromDb.DateOrdered = chinaOrder.DateOrdered;
                objFromDb.DateReceived = chinaOrder.DateReceived;
                objFromDb.QuantityReceived = chinaOrder.QuantityReceived;
                objFromDb.BoxCount = chinaOrder.BoxCount;
                objFromDb.IgnoreMissingQuantity = chinaOrder.IgnoreMissingQuantity;
                objFromDb.ReceivedAll = chinaOrder.ReceivedAll;
                objFromDb.KTMerchId = chinaOrder.KTMerchId;
            }
        }
    }
}
