using KTSite.Models;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IComplaintsArchiveRepository :IRepository<ComplaintsArchive>
    {
        ComplaintsArchive Get(long id);
    }
}
