using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class AdminVATaskRepository : Repository<AdminVATask> , IAdminVATaskRepository
    {
        private readonly ApplicationDbContext _db;
        public AdminVATaskRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(AdminVATask adminVATask)
        {
            var objFromDb = _db.adminVaTasks.FirstOrDefault(s=>s.Id == adminVATask.Id);
            if (objFromDb != null)
            {
                objFromDb.StoreId = adminVATask.StoreId;
                objFromDb.UserNameId = adminVATask.UserNameId;
                objFromDb.DateCreated = adminVATask.DateCreated;
                objFromDb.TaskToDo = adminVATask.TaskToDo;
                objFromDb.TaskCompleted = adminVATask.TaskCompleted; 
            }
        }
    }
}
