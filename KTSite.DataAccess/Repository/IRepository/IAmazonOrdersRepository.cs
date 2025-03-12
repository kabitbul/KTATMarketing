using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAmazonOrdersRepository
    {
       public List<SkuQtyForAverage> GetAllOrdersForAvg(string marketPlace);
       //public List<SkuQtyForAverage> GetAllWebsiteOrdersForAvg();
       public List<GraphData> GetGraphData(string marketPlace, string asin);
       public List<GraphData> GetTotalOrdGraphData(string marketPlace);
    }
}
