using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface INotificationRepository : IRepository<Notification>
    {
        //public IEnumerable<Order> getAllOrdersOfUser(string userNameId);
        void update(Notification notification);
        //ReturnLabel Get(long id);
    }
}
