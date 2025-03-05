using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAsinToSkuRepository
    {
       public List<AsinToSku> GetList();
       public AsinToSku GetById(int id);
       public string GetSkuByAsin(string asin);
       public int updateRestockStatus(string sql);
       public int updateById(int id , string asin, string sku, string chinaName, string imageUrl,
                              bool restockNOTDECIDED, bool IsCanadaAsin);
       public bool InsertAsinToSku(string asin, string sku, string chinaName,string imageUrl);
       public bool DeleteById(int id);
       
    }
}
