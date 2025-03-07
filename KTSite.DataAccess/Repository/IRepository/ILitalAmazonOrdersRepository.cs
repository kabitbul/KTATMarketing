using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface ILitalAmazonOrdersRepository
    {
       public List<SkuQtyForAverage> GetAllOrdersForAvg(string marketPlace);
       
       public List<GraphData> GetGraphData(string marketPlace, string asin);
    }
}
