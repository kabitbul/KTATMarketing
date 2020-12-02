using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class ReturningItemRepository : Repository<ReturningItem> , IReturningItemRepository
    {
        private readonly ApplicationDbContext _db;
        public ReturningItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(ReturningItem returningItem)
        {
            var objFromDb = _db.ReturningItems.FirstOrDefault(s=>s.Id == returningItem.Id);
            if (objFromDb != null)
            {
                objFromDb.ProductId = returningItem.ProductId;
                objFromDb.ItemStatus = returningItem.ItemStatus;
                objFromDb.Quantity = returningItem.Quantity;
                //objFromDb.UserStoreName = order.UserStoreName;
                objFromDb.Comments = returningItem.Comments;
                objFromDb.DateArrived = returningItem.DateArrived;
                objFromDb.CreatedBy = returningItem.CreatedBy;
            }
        }
    }
}
