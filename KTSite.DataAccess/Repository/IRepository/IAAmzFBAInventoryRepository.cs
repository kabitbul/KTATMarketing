using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAAmzFBAInventoryRepository
    {
      public List<AmazonInvStatistics> inventoryIndexData(bool? showRestock, string marketplace,int storeId);
      public List<AmazonInvStatistics> GetInventoryStat(string marketPlace,int storeId,bool? showRestock);

      public int getAvg(int days, string asin, List<SkuQtyForAverage> lst, bool isWebsite);
      public int last30daysSales(string asin, List<SkuQtyForAverage> lst);
      public bool needToOrderFromChina(AmazonInvStatistics obj,double dailySales, int onTheWay);
    }
}
