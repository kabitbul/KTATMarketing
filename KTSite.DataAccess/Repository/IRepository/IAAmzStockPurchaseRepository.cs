using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAAmzStockPurchaseRepository
    {
     public List<AAmzStockPurchase> GetList(int storeId,string marketplace);
     public AAmzStockPurchase GetById(int id);
     public bool AddStock(AAmzStockPurchaseVM aAmzStockPurchaseVM);
     public int updateById(AAmzStockPurchaseVM aAmzStockPurchaseVM);
       
    }
}
