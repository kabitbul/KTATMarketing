using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class ReturnLabelRepository : Repository<ReturnLabel> , IReturnLabelRepository
    {
        private readonly ApplicationDbContext _db;
        public ReturnLabelRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<Order> getAllOrdersOfUser(string userNameId)
        {
            IEnumerable<Order> orderList = _db.Orders.FromSqlRaw("select * from Orders o where UserNameId = '" +
                userNameId + "' and o.OrderStatus = '"+SD.OrderStatusDone+"' and not exists(select 1 from returnLabels r where r.OrderId = o.Id)");
            return orderList;
        }
        public void update(ReturnLabel returnLabel)
        {
            var objFromDb = _db.returnLabels.FirstOrDefault(s=>s.Id == returnLabel.Id);
            if (objFromDb != null)
            {
                objFromDb.OrderId = returnLabel.OrderId;
                objFromDb.UserNameId = returnLabel.UserNameId;
                objFromDb.FileName = returnLabel.FileName;
                objFromDb.FileURL = returnLabel.FileURL;
                objFromDb.CommentsToWarehouse = returnLabel.CommentsToWarehouse;
                objFromDb.DateCreated = returnLabel.DateCreated;
                objFromDb.ReturnTracking = returnLabel.ReturnTracking;
                objFromDb.ReturnDelivered = returnLabel.ReturnDelivered;
                objFromDb.ReturnQuantity = returnLabel.ReturnQuantity;
            }
        }
    }
}
