using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface ILogsDataRepository : IRepository<LogsData>
    {
        void update(LogsData logsData);
    }
}
