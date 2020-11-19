using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class UserGuidelineRepository : Repository<UserGuideline> , IUserGuidelineRepository
    {
        private readonly ApplicationDbContext _db;
        public UserGuidelineRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(UserGuideline userGuideline)
        {
            var objFromDb = _db.UserGuidelines.FirstOrDefault(s=>s.Id == userGuideline.Id);
            if (objFromDb != null)
            {
                objFromDb.Guideline = userGuideline.Guideline;
            }
        }
    }
}
