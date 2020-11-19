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
    public class ComplaintsRepository : Repository<Complaints> , IComplaintsRepository
    {
        private readonly ApplicationDbContext _db;
        public ComplaintsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<Order> getAllOrdersOfUser(string userNameId)
        {
            IEnumerable<Order> orderList = _db.Orders.FromSqlRaw("select * from Orders o where UserNameId = '" +
                userNameId + "' and o.OrderStatus = '" + SD.OrderStatusDone + "' and not exists(select 1 from Complaints c where c.OrderId = o.Id)");
            return orderList;
        }
        public IEnumerable<Order> getAllOrdersForAdmin()
        {
            IEnumerable<Order> orderList = _db.Orders.FromSqlRaw("select * from Orders o where " +
                " o.OrderStatus = '" + SD.OrderStatusDone + "' and not exists(select 1 from Complaints c where c.OrderId = o.Id)");
            return orderList;
        }
        public void update(Complaints complaints)
        {
            var objFromDb = _db.Complaints.FirstOrDefault(s=>s.Id == complaints.Id);
            if (objFromDb != null)
            {
                objFromDb.OrderId = complaints.OrderId;
                objFromDb.UserNameId = complaints.UserNameId;
                objFromDb.Description = complaints.Description;
                objFromDb.SolutionDesc = complaints.SolutionDesc;
                objFromDb.HandledBy = complaints.HandledBy;
                objFromDb.Solved = complaints.Solved;
                objFromDb.CreatedDate = complaints.CreatedDate;
                objFromDb.NewTrackingNumber = complaints.NewTrackingNumber;
            }
        }
    }
}
