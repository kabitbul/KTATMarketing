using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface ILitalAmazonInventoryRepository
    {
       public List<LitalAmazonInvStatistics> GetInventoryStat(string marketPlace, bool? showRestock);
    }
}
