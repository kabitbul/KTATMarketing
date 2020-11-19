using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class SellersInventoryRepository : Repository<SellersInventory> , ISellersInventoryRepository
    {
        private readonly ApplicationDbContext _db;
        public SellersInventoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(SellersInventory sellersInventory)
        {
            var objFromDb = _db.SellersInventories.FirstOrDefault(s=>s.Id == sellersInventory.Id);
            if (objFromDb != null)
            {
                objFromDb.ProductId = sellersInventory.ProductId;
                objFromDb.UserNameId = sellersInventory.UserNameId;
                objFromDb.UserName = sellersInventory.UserName;
                objFromDb.StoreNameId = sellersInventory.StoreNameId;
            }
        }
    }
}
