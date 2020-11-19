using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class NotificationRepository : Repository<Notification> , INotificationRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Notification notification)
        {
            var objFromDb = _db.Notifications.FirstOrDefault(s=>s.Id == notification.Id);
            if (objFromDb != null)
            {
                objFromDb.Message = notification.Message;
                objFromDb.HandledBy = notification.HandledBy;
                objFromDb.Visible = notification.Visible;
                objFromDb.DateMsg = notification.DateMsg;
            }
        }
    }
}
