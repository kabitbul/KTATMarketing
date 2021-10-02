using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IReturningItemArchiveRepository : IRepository<ReturningItemArchive>
    {
        ReturningItemArchive Get(long id);
    }
}
