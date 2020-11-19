using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IChinaOrderRepository : IRepository<ChinaOrder>
    {
        void update(ChinaOrder chinaOrder);
        ChinaOrder Get(long id);
    }
}
