using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System.Linq;

namespace KTSite.DataAccess.Repository
{
    public class UsersForAPIRepository : Repository<UsersForAPI> , IUsersForAPIRepository
    {
        private readonly ApplicationDbContext _db;
        public UsersForAPIRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(UsersForAPI usersForAPI)
        {
            var objFromDb = _db.UsersForAPIs.FirstOrDefault(s=>s.Id == usersForAPI.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = usersForAPI.Name;
                objFromDb.AuthToken = usersForAPI.AuthToken;
                objFromDb.CreatedDate = usersForAPI.CreatedDate;
                objFromDb.Active = usersForAPI.Active;
            }
        }
    }
}
