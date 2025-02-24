using Dapper;
using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KTSite.DataAccess.Repository
{
    public class AmazonOrdersRepository :  IAmazonOrdersRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        public AmazonOrdersRepository(IConfiguration configuration)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public List<SkuQtyForAverage> GetAllOrdersForAvg(string marketPlace)
        {
         string startDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow.AddDays(-32),
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date.ToString("yyyy-MM-dd HH:mm:ss");
       string endDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date.ToString("yyyy-MM-dd HH:mm:ss");
            var sql =
                "SELECT sk.Sku, o.Qty,o.PurchaseDate "+
"                FROM AmazonOrders o , AsinToSku sk " +
"                WHERE MarketPlace = '"+marketPlace+"' AND " +
"                      o.Asin = sk.Asin AND "+
"                      o.PurchaseDate >=  '"+startDate+"' AND" +
"                      o.PurchaseDate <= '"+endDate+"'";

            List<SkuQtyForAverage> ordList = _db.Query<SkuQtyForAverage>(sql).ToList();
            return ordList;
        } 
//        public List<SkuQtyForAverage> GetAllWebsiteOrdersForAvg()
//        {
//         string startDate = DateTime.Now.AddDays(-4).Date.ToString("yyyy-MM-dd HH:mm:ss");
                          
//       string endDate = DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss");
//            var sql =
//                "SELECT o.ProductName Sku, o.Quantity Qty,o.UsDate PurchaseDate"+
//"                FROM Orders o " +
//"                WHERE o.UsDate >=  '"+startDate+"' AND" +
//"                      o.UsDate <= '"+endDate+"' AND " +
//"                      o.OrderStatus <> 'Cancelled'";

//            List<SkuQtyForAverage> ordList = _db.Query<SkuQtyForAverage>(sql).ToList();
//            return ordList;
//        } 
    }
}
