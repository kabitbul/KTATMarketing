using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class LogsDataRepository : Repository<LogsData> , ILogsDataRepository
    {
        private readonly ApplicationDbContext _db;
        public LogsDataRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(LogsData logsData)
        {
            var objFromDb = _db.LogsDatas.FirstOrDefault(s=>s.Id == logsData.Id);
            if (objFromDb != null)
            {
                objFromDb.Msg1 = logsData.Msg1;
                objFromDb.Msg2 = logsData.Msg2;
                objFromDb.CreatedDate = logsData.CreatedDate;
                objFromDb.CreatedBy = logsData.CreatedBy;
            }
        }
    }
}
