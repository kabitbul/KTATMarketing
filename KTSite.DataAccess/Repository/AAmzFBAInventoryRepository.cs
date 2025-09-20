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
using System.Text.RegularExpressions;

namespace KTSite.DataAccess.Repository
{
    public class AAmzFBAInventoryRepository :  IAAmzFBAInventoryRepository
    {
        //private readonly ApplicationDbContext _db;
        private IDbConnection _db;
        private IAAmzOrdersRepository _amazonOrdersRepository;
        private IAAmzAsinToSkuRepository _amazonAsinToSkuRepository;
        
        public AAmzFBAInventoryRepository(IConfiguration configuration, IAAmzOrdersRepository amzord,
                                                           IAAmzAsinToSkuRepository asku)
        {
            _db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
           _amazonOrdersRepository = amzord;
           _amazonAsinToSkuRepository = asku;
        }
        public List<AmazonInvStatistics> inventoryIndexData(bool? showRestock, string marketplace,int storeId)
        {
           int avg14daysForCalcOOS = 0;
          List<AmazonInvStatistics> invList = 
                  GetInventoryStat(marketplace,storeId,showRestock);
          List<SkuQtyForAverage> skuQtyForAverage = 
           _amazonOrdersRepository.GetAllOrdersForAvg(marketplace,storeId);
         //List<SkuQtyForAverage> websiteOrdersForAverage = 
           //_unitOfWork.amazonOrders.GetAllWebsiteOrdersForAvg();
          List<AAmzAsinToSku> lst = _amazonAsinToSkuRepository.GetList(storeId);

           int count = 1;
            foreach(AmazonInvStatistics obj in invList)
            {
              
               count++;
               obj.avg3days = getAvg(3,obj.Asin,skuQtyForAverage, false); 
               //obj.avg7days = getAvg(7,obj.Asin,skuQtyForAverage, false);
               obj.avgMonth = getAvg(30,obj.Asin,skuQtyForAverage, false);
               avg14daysForCalcOOS = getAvg(14,obj.Asin,skuQtyForAverage, false);
               obj.avg14days = avg14daysForCalcOOS; 
               obj.sales30Days = last30daysSales(obj.Asin,skuQtyForAverage);
               //obj.avg3daysEbay = getAvg(3,obj.sku,websiteOrdersForAverage,true); 
               if(avg14daysForCalcOOS == 0)
               { 
                avg14daysForCalcOOS = 10000;
                obj.needToOrderFromChina = false;
                obj.daysToOOS = 10000;
               }
               else{ 
                    obj.daysToOOS = 
            (obj.AmzAvailQty+obj.AmzInboundQty + obj.AmzAWDAvailQty + obj.AmzAWDInboundQty + obj.onTheWay) /(avg14daysForCalcOOS);
                    
                    
                    
                         obj.needToOrderFromChina = needToOrderFromChina(obj,avg14daysForCalcOOS,obj.onTheWay);
                  // obj.needToOSendFromWarehouse = needToSendFromWarehouse(obj,
                                                            //          (obj.avg3days));
                 }
              
            }
            return invList;    
        }
   public List<AmazonInvStatistics> GetInventoryStat(string marketPlace,int storeId,bool? showRestock)
        {
            string sql;
            bool shr;
            if (showRestock == null || showRestock == true)
                 shr = true;
            else
             shr = false;
           if (marketPlace == "US")
            { 
    sql =
"SELECT sk.Id,sk.ImageUrl, sk.Sku, sk.Asin, sk.ChinaName, "+
"            inv.AvailableQty AmzAvailQty, "+
"            (inv.InboundReceivingQty + inv.InboundShippedQty + inv.ReservedQty) AmzInboundQty, "+
"            aw.totalInboundQuantity AmzAWDInboundQty, aw.totalOnhandQuantity AmzAWDAvailQty, "+
"            sk.RestockUS restock, sk.RestockCA restockCA,sk.RestockNOTDECIDED, "+
"			COALESCE((select sum(Quantity) from AAmzStockPurchase sp "+
"			     where sp.ProductAsin = inv.Asin and sp.InboundUpdated = 0 and MarketPlace = '"+marketPlace+"'" +
"                  and storeId = "+ storeId +"),0) onTheWay "+
"     FROM AAmzFBAInventory inv JOIN AAmzAsinToSku sk ON inv.Asin = sk.Asin left join AAmzAWDInventory aw on aw.Asin = sk.Asin"+
"     WHERE inv.MarketPlace = '"+marketPlace+"' AND inv.StoreId = " + storeId;
      if(shr)
       { 
     sql = sql + " AND RestockUS = 1 ";
        }    
}
else{//CA
sql =
"SELECT sk.Id,sk.ImageUrl, sk.Sku, sk.Asin, sk.ChinaName,"+
"            inv.AvailableQty AmzAvailQty, "+
"            (inv.InboundReceivingQty + inv.InboundShippedQty + inv.ReservedQty) AmzInboundQty, "+
"            sk.RestockUS restockUS, sk.RestockCA restock, sk.RestockNOTDECIDEDCA, "+
"			COALESCE((select sum(Quantity) from AAmzStockPurchase sp where sp.ProductAsin = inv.Asin and sp.InboundUpdated = 0" +
"            and MarketPlace = '"+marketPlace+"' and storeId = "+ storeId +"),0) onTheWay "+
"     FROM AAmzFBAInventory inv JOIN AAmzAsinToSku sk ON inv.Asin = sk.Asin "+
"     WHERE inv.MarketPlace = '"+marketPlace+"' and sk.IsCanadaAsin = 1 AND inv.StoreId = " + storeId;
if(shr)
       { 
     sql = sql + " AND RestockCA = 1 ";
        }
            }
     
            return _db.Query<AmazonInvStatistics>(sql).ToList();
        }
 public int getAvg(int days, string asin, List<SkuQtyForAverage> lst, bool isWebsite)
        {
         
        DateTime startDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow.AddDays(-days),
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date;
         DateTime endDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date;
         if(isWebsite)
           {
             startDate = DateTime.Now.AddDays(-days).Date;
            endDate = DateTime.Now.Date;

            }
           double totalQty = lst
            .Where(a => a.Asin == asin && a.PurchaseDate >= startDate && a.PurchaseDate <= endDate
                 ).Sum(a => a.Qty);
         return (int)Math.Floor(totalQty/days);
        }
 public int last30daysSales(string asin, List<SkuQtyForAverage> lst)
        {
         
        DateTime startDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow.AddDays(-30),
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date;
         DateTime endDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
                              TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                          .Date;
         
           int totalQty = lst
            .Where(a => a.Asin == asin && a.PurchaseDate >= startDate && a.PurchaseDate <= endDate
                 ).Sum(a => a.Qty);
            return totalQty;
        
        }
public bool needToOrderFromChina(AmazonInvStatistics obj,double dailySales, int onTheWay)
{
//if number of items that expected to be sold is less then
// our total inventory in watrhouse + amazon + on the way
   if((SD.amzChinaShipDays*dailySales) >= 
              (obj.AmzAvailQty + obj.AmzInboundQty+ obj.AmzAWDAvailQty + obj.AmzAWDInboundQty + onTheWay))
             {
//need to order - but if there is already a line with this asin on china order - and the inboundUpdated is false
             // bool inboundUpd = _unitOfWork.inventoryOrdersToAmazon.getInboundUpdated(obj.Asin);
             //  if (!inboundUpd)
               //   return false;
               return true;
             }
            return false;
          }
//--///////////////////////////////////////////////////////////////////////////////

     
    }
}
