using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IReturnLabelArchiveRepository : IRepository<ReturnLabelArchive>
    {
        ReturnLabelArchive Get(long id);
    }
}
