using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.DataAccess.Repository.IRepository
{
    public interface ISellersInventoryRepository :IRepository<SellersInventory>
    {
        void update(SellersInventory sellersInventory);
    }
}
