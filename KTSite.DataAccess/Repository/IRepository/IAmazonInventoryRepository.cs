using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAmazonInventoryRepository
    {
       public List<AmazonInvStatistics> GetInventoryStat(string marketPlace);
    }
}
