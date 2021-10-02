using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;

namespace KTSite.DataAccess.Repository
{
    public class ReturningItemArchiveRepository : Repository<ReturningItemArchive> , IReturningItemArchiveRepository
    {
        private readonly ApplicationDbContext _db;
        public ReturningItemArchiveRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
