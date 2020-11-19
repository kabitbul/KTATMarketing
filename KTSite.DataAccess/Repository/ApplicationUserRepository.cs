using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser> , IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<ApplicationUser> GeAllUsersWithoutrecInPayBalance()
        {
            IEnumerable<ApplicationUser> ApplicationUserList = _db.ApplicationUser.FromSqlRaw("select * from AspNetUsers u where " +
                "not exists (select 1 from PaymentBalances p where UserNameId = u.Id)");
            return ApplicationUserList;
        }
        
        public void update(ApplicationUser applicationUser)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(s => s.Id == applicationUser.Id);
            if (objFromDb != null)
            {
                objFromDb.LockoutEnd = applicationUser.LockoutEnd;
            }
        }
    }
}
