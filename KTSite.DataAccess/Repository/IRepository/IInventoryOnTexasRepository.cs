using KTSite.Models;
using System.Collections.Generic;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface IInventoryOnTexasRepository
    {
       public List<InventoryOnTexasSumList> GetInventoryOnTexasSum();
       
    }
}
