using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAdminVATaskRepository :IRepository<AdminVATask>
    {
        void update(AdminVATask adminVATask);
        AdminVATask Get(int id);
    }
}
