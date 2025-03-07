using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface ILitalAsinToSkuRepository
    {
       public List<LitalAsinToSku> GetList();
       public LitalAsinToSku GetById(int id);
       public int updateRestockStatus(string sql);
       public int updateById(int id , string asin,  string chinaName, string imageUrl,
                              bool restockNOTDECIDED);
       public bool InsertAsinToSku(string asin,  string chinaName,string imageUrl);
       public bool DeleteById(int id);
       
    }
}
