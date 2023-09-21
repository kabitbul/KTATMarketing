using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace KTSite.DataAccess.Repository
{
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<Product> getAllProducts(int id)
        {
            IEnumerable<Product> productList =  _db.Products.FromSqlRaw("select * from Products Where Id =" +id.ToString());
            return productList;
        }
        public void update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(s=>s.Id == product.Id);
            if (objFromDb != null)
            {
                if(product.ImageUrl != null)
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }
                objFromDb.ProductName = product.ProductName;
                objFromDb.Cost = product.Cost;
                objFromDb.SellersCost = product.SellersCost;
                objFromDb.ShippingCharge = product.ShippingCharge;
                objFromDb.Weight = product.Weight;
                objFromDb.OnTheWayInventory = product.OnTheWayInventory;
                objFromDb.ReStock = product.ReStock;
                objFromDb.OwnByWarehouse = product.OwnByWarehouse;
                objFromDb.AvailableForSellers = product.AvailableForSellers;
                objFromDb.CategoryId = product.CategoryId;
                objFromDb.ProductDesc = product.ProductDesc;
                objFromDb.OOSForSellers = product.OOSForSellers;
                objFromDb.MadeIn = product.MadeIn;
                objFromDb.IsFBA = product.IsFBA;
                objFromDb.ImageUrl = product.ImageUrl;
                objFromDb.ProductASIN = product.ProductASIN;
                objFromDb.ProductURL = product.ProductURL;
                objFromDb.BestOffer = product.BestOffer;
                objFromDb.MinimumPrice = product.MinimumPrice;
                objFromDb.WarehouseChinaCost = product.WarehouseChinaCost;
                objFromDb.AdminApproval = product.AdminApproval;
                objFromDb.RejectReason = product.RejectReason;
                objFromDb.PlatformsPrice = product.PlatformsPrice;
                if(product.MerchType == SD.Role_ExMerch)
                {
                    objFromDb.InventoryCount = product.InventoryCount;
                }

            }
        }
    }
}
