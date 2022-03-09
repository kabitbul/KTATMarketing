using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System.Linq;

namespace KTSite.DataAccess.Repository
{
    public class ExcelUploadsForShopsRepository : Repository<ExcelUploadsForShops> , IExcelUploadsForShopsRepository
    {
        private readonly ApplicationDbContext _db;
        public ExcelUploadsForShopsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void update(ExcelUploadsForShops excelUploadsForShops)
        {
            var objFromDb = _db.ExcelUploadsForShopss.FirstOrDefault(s => s.Id == excelUploadsForShops.Id);
            if (objFromDb != null)
            {
                objFromDb.UserId = excelUploadsForShops.UserId;
                objFromDb.FromOrderId = excelUploadsForShops.FromOrderId;
                objFromDb.ToOrderId = excelUploadsForShops.ToOrderId;
                objFromDb.TrackingUpdated = excelUploadsForShops.TrackingUpdated;
                objFromDb.CreatedDate = excelUploadsForShops.CreatedDate;
            }
        }
    }
}
