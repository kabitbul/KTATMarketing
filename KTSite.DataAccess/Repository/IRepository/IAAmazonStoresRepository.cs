using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IAAmazonStoresRepository
    {
       public List<AAmazonStores> GetList();
       public AAmazonStores GetById(int id);
       public int updateById(int id , string storeName);
       public bool InsertStore(string storeName);
       public bool DeleteById(int id);
       
    }
}
