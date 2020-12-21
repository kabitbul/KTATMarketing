using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class OrderRepository : Repository<Order> , IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Order order)
        {
            var objFromDb = _db.Orders.FirstOrDefault(s=>s.Id == order.Id);
            if (objFromDb != null)
            {
                objFromDb.ProductId = order.ProductId;
                objFromDb.UserNameId = order.UserNameId;
                objFromDb.StoreNameId = order.StoreNameId;
                //objFromDb.UserStoreName = order.UserStoreName;
                objFromDb.Quantity = order.Quantity;
                objFromDb.CustName = order.CustName;
                objFromDb.CustStreet1 = order.CustStreet1;
                objFromDb.CustStreet2 = order.CustStreet2;
                objFromDb.CustCity = order.CustCity;
                objFromDb.CustState = order.CustState;
                objFromDb.CustZipCode = order.CustZipCode;
                objFromDb.CustPhone = order.CustPhone;
                objFromDb.Cost = order.Cost;
                objFromDb.Carrier = order.Carrier;
                objFromDb.TrackingNumber = order.TrackingNumber;
                objFromDb.IsAdmin = order.IsAdmin;
                objFromDb.OrderStatus = order.OrderStatus;
                objFromDb.ProductName = order.ProductName;
                objFromDb.UserNameToShow = order.UserNameToShow;
                objFromDb.StoreName = order.StoreName;
                objFromDb.TrackingUpdated = order.TrackingUpdated;
            }
        }
    }
}
