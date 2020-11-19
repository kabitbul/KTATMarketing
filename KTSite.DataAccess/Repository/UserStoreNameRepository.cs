using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class UserStoreNameRepository : Repository<UserStoreName> , IUserStoreNameRepository
    {
        private readonly ApplicationDbContext _db;
        public UserStoreNameRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(UserStoreName userStoreName)
        {
            var objFromDb = _db.UserStoreNames.FirstOrDefault(s=>s.Id == userStoreName.Id);
            if (objFromDb != null)
            {
                objFromDb.UserNameId = userStoreName.UserNameId;
                objFromDb.StoreName = userStoreName.StoreName;
                objFromDb.UserName = userStoreName.UserName;
                objFromDb.IsAdminStore = userStoreName.IsAdminStore;
            }
        }
    }
}
