using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAAmzAsinToSkuRepository
    {
       public List<AAmzAsinToSku> GetList(int store);
       public List<AAmzAsinToSku> GetListByMarketplace(int store,string marketplace);
      public AAmzAsinToSku getForUpsert(int Id,int storeId);
      public bool Upsert(AAmzAsinToSku aAmzAsinToSku,int storeId);
        public AAmzAsinToSku GetById(int id, int storeId);
       public string GetSkuByAsin(string asin);
       public int updateRestockStatus(string sql);
       public int updateById(int storeId,int id , string asin, string sku, string chinaName, string imageUrl,
                              bool restockNOTDECIDED, bool IsCanadaAsin);
       public bool InsertAsinToSku(int storeId,string asin, string sku, string chinaName, string imageUrl,bool IsCanadaAsin);
       public bool DeleteById(int id,int storeId);
       
    }
}
