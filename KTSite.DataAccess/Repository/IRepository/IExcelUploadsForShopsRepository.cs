using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IExcelUploadsForShopsRepository : IRepository<ExcelUploadsForShops>
    {
        void update(ExcelUploadsForShops excelUploadsForShops);
    }
}
