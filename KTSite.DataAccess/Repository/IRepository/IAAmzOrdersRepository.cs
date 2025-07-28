using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAAmzOrdersRepository
    {
      public List<SkuQtyForAverage> GetAllOrdersForAvg(string marketPlace,int storeId);
      public List<GraphData> GetGraphData(int storeId , string marketPlace, string asin);
      public List<GraphData> GetTotalOrdGraphData(string marketPlace,int storeId);
       
    }
}
