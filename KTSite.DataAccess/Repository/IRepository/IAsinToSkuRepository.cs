using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAsinToSkuRepository
    {
       public List<AsinToSku> GetList();
       public AsinToSku GetById(int id);
       public int updateRestockStatus(string sql);
       public int updateById(int id , string asin, string sku, string chinaName);
       public bool InsertAsinToSku(string asin, string sku, string chinaName);
       
    }
}
