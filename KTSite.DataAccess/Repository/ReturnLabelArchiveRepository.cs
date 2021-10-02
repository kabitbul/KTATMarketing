using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;

namespace KTSite.DataAccess.Repository
{
    public class ReturnLabelArchiveRepository : Repository<ReturnLabelArchive> , IReturnLabelArchiveRepository
    {
        private readonly ApplicationDbContext _db;
        public ReturnLabelArchiveRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
