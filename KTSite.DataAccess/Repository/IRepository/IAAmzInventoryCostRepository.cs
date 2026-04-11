using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAAmzInventoryCostRepository
    {
       public List<AAmzInventoryCost> GetList(int storeId, string marketplace, int days);
       public  List<AAmzInventoryCost> GetByLastDate(int storeId);
      
      
      
       
    }
}
