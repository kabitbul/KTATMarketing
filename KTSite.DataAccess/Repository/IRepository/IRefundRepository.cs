using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IRefundRepository :IRepository<Refund>
    {
        void update(Refund refund);
        Refund Get(long id);
    }
}
