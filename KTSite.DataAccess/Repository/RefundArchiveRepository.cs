using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;

namespace KTSite.DataAccess.Repository
{
    public class RefundArchiveRepository : Repository<RefundArchive> , IRefundArchiveRepository
    {
        private readonly ApplicationDbContext _db;
        public RefundArchiveRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
