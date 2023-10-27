using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAsinToSkuRepository
    {
       public List<AsinToSku> GetList();
       public int updateRestockStatus(string sql);
       public bool InsertAsinToSku(string asin, string sku);
       
    }
}
