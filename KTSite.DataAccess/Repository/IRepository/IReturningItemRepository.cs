using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IReturningItemRepository : IRepository<ReturningItem>
    {
        void update(ReturningItem returningItem);
        ReturningItem Get(long id);
    }
}
