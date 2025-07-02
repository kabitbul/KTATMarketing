using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class LitalAmazonInventoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public LitalAmazonInventoryController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(bool? showRestock )
        {
          List<LitalAmazonInvStatistics> invList = 
                  _unitOfWork.litalAmazonInventories.GetInventoryStat("US",showRestock);
          List<SkuQtyForAverage> skuQtyForAverage = 
           _unitOfWork.litalAmazonOrders.GetAllOrdersForAvg("US");
          List<LitalAsinToSku> lst = _unitOfWork.litalAsinToSku.GetList();
           int count = 1;
            foreach(LitalAmazonInvStatistics obj in invList)
            {
              
               count++;
               obj.avg3days = getAvg(3,obj.Asin,skuQtyForAverage, false); 
               obj.avg14days = getAvg(14,obj.Asin,skuQtyForAverage, false);
               obj.avgMonth = getAvg(30,obj.Asin,skuQtyForAverage, false);
               obj.sales30Days = last30daysSales(obj.Asin,skuQtyForAverage);
               if(obj.avg14days == 0)
               { 
                obj.daysToOOS = 10000;
                obj.needToOrderFromChina = false;
                //obj.needToOSendFromWarehouse = false;
               }
               else{ 
                   if(obj.avg14days == 0)
                      obj.daysToOOS = 10000;
                   else
                    obj.daysToOOS = (obj.AmzAvailQty+obj.AmzInboundQty + obj.onTheWay) /(obj.avg14days);
                    
                    
                    
                         obj.needToOrderFromChina = needToOrderFromChina(obj,obj.avg14days,obj.onTheWay);
                  // obj.needToOSendFromWarehouse = needToSendFromWarehouse(obj,
                                                            //          (obj.avg3days));
                 }
              
            }
            return View(invList);
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
         public bool needToOrderFromChina(LitalAmazonInvStatistics obj,double dailySales, int onTheWay)
          {
             //if number of items that expected to be sold is less then
            // our total inventory in watrhouse + amazon + on the way
             if((SD.amzChinaShipDays*dailySales) >= (obj.AmzAvailQty + obj.AmzInboundQty+ onTheWay))
             {
//need to order - but if there is already a line with this asin on china order - and the inboundUpdated is false
             // bool inboundUpd = _unitOfWork.inventoryOrdersToAmazon.getInboundUpdated(obj.Asin);
             //  if (!inboundUpd)
               //   return false;
               return true;
             }
            return false;
          }

        [HttpPost]
        public IActionResult UpdateRestockND(int id, bool restockNotDecided)
          {
           string sql = "";
           if(id > 0)
            {
              sql = " UPDATE LitalAsinToSku " +
  "                               SET RestockNOTDECIDED = "+(restockNotDecided ? 1:0)+" " +
  "                               WHERE Id = " +id+";"; 
              int res = _unitOfWork.litalAsinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }

       [HttpPost]
        public IActionResult UpdateRestockUS(int id, bool restock)
        {
            string sql = "";
           if(id > 0)
            {
              sql = " UPDATE LitalAsinToSku " +
  "                               SET Restock = "+(restock ? 1:0)+" " +
  "                               WHERE Id = " +id+";"; 
              int res = _unitOfWork.litalAsinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll();
            return Json(new { data = allObj });
        }

        #endregion
    }
}
