using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface ILogsDataRepository : IRepository<LogsData>
    {
        void update(LogsData logsData);
    }
}
