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
    public class AmazonInventoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AmazonInventoryController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(bool? showRestock )
        {
          int avg14daysForCalcOOS = 0;
          List<AmazonInvStatistics> invList = 
                  _unitOfWork.amazonInventories.GetInventoryStat("US",showRestock);
          List<SkuQtyForAverage> skuQtyForAverage = 
           _unitOfWork.amazonOrders.GetAllOrdersForAvg("US");
         //List<SkuQtyForAverage> websiteOrdersForAverage = 
           //_unitOfWork.amazonOrders.GetAllWebsiteOrdersForAvg();
          List<AsinToSku> lst = _unitOfWork.asinToSku.GetList();
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
                //obj.needToOSendFromWarehouse = false;
               }
               else{ 
                   if(avg14daysForCalcOOS == 0)
                      obj.daysToOOS = 10000;
                   else
                    obj.daysToOOS = (obj.AmzAvailQty+obj.AmzInboundQty + obj.onTheWay) /(avg14daysForCalcOOS);
                    
                    
                    
                         obj.needToOrderFromChina = needToOrderFromChina(obj,avg14daysForCalcOOS,obj.onTheWay);
                  // obj.needToOSendFromWarehouse = needToSendFromWarehouse(obj,
                                                            //          (obj.avg3days));
                 }
              
            }
            return View(invList);
        }
        public IActionResult CAInv(bool showRestock = true)
        {
            int avg14daysForCalcOOS = 0;
                List<AmazonInvStatistics> invList = 
                  _unitOfWork.amazonInventories.GetInventoryStat("CA",showRestock);
          List<SkuQtyForAverage> skuQtyForAverage = 
           _unitOfWork.amazonOrders.GetAllOrdersForAvg("CA");

          List<AsinToSku> lst = _unitOfWork.asinToSku.GetList();
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
               if(obj.avg3days == 0)
               { 
                obj.daysToOOS = 10000;
                obj.needToOrderFromChina = false;
                //obj.needToOSendFromWarehouse = false;
               }
               else{ 
                   if(avg14daysForCalcOOS == 0)
                      obj.daysToOOS = 10000;
                   else
                    obj.daysToOOS = (obj.AmzAvailQty+obj.AmzInboundQty + obj.onTheWay) /(avg14daysForCalcOOS);
                    
                    
                    
                         obj.needToOrderFromChina = needToOrderFromChina(obj,avg14daysForCalcOOS,obj.onTheWay);
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
         public bool needToOrderFromChina(AmazonInvStatistics obj,double dailySales, int onTheWay)
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
          //public bool needToSendFromWarehouse(AmazonInvStatistics obj,double dailySales)
          //{
          //   //if number of items that expected to be sold is less then
          //  // our total inventory in watrhouse + amazon + on the way
          //   if((SD.amzWarehouseShipDays*dailySales) >= (obj.AmzAvailQty +
          //                                               obj.AmzInboundQty))
            
          //    return true;
          //  return false;
          //}
        [HttpPost]
        public IActionResult UpdateRestockND(int id, bool restockNotDecided)
          {
           string sql = "";
           if(id > 0)
            {
              sql = " UPDATE AsinToSku " +
  "                               SET RestockNOTDECIDED = "+(restockNotDecided ? 1:0)+" " +
  "                               WHERE Id = " +id+";"; 
              int res = _unitOfWork.asinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }
[HttpPost] 
        public IActionResult UpdateRestockNDCA(int id, bool restockNotDecidedCA)
          {                                                 
           string sql = "";
           if(id > 0)
            {
              sql = " UPDATE AsinToSku " +
  "                               SET RestockNOTDECIDEDCA = "+(restockNotDecidedCA ? 1:0)+" " +
  "                               WHERE Id = " +id+";"; 
              int res = _unitOfWork.asinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }
       [HttpPost]
        public IActionResult UpdateRestockUS(int id, bool restock)
        {
            string sql = "";
           if(id > 0)
            {
              sql = " UPDATE AsinToSku " +
  "                               SET RestockUS = "+(restock ? 1:0)+" " +
  "                               WHERE Id = " +id+";"; 
              int res = _unitOfWork.asinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }
[HttpPost]
        public IActionResult UpdateRestockCA(int id, bool restock)
        {
            string sql = "";
           if(id > 0)
            {
              sql = " UPDATE AsinToSku " +
  "                               SET RestockCA = "+(restock ? 1:0)+" " +
  "                               WHERE Id = " +id+";"; 
              int res = _unitOfWork.asinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }
        [HttpPost]
        public IActionResult UpdateInvCA( string model, string  arrId, string checkedVal)
        {
            IEnumerable<AmazonInvStatistics> modelData = 
              JsonConvert.DeserializeObject<IEnumerable<AmazonInvStatistics>>(model);
            string[] arrIdStrings = arrId.Split(',');
            int[] Ids = arrIdStrings.Select(int.Parse).ToArray();
            string[] valStrings = checkedVal.Split(',');
            bool[] Vals = valStrings.Select(bool.Parse).ToArray();
             string sql = "";
            foreach (AmazonInvStatistics obj in modelData)
            {
                if(!Ids.Contains(obj.Id))
                  continue;
 
                bool restock = obj.restockCA;
                int index = Array.IndexOf(Ids, obj.Id);
                if ((bool)obj.restockCA != Vals[index])
                 {
                    if(Vals[index])
                     {
                       sql = sql + " UPDATE AsinToSku " +
  "                               SET RestockCA = 1 " +
  "                               WHERE Asin = '" + obj.Asin + "'; "; 
                     }
                     else
                      {
                       sql = sql + " UPDATE AsinToSku " +
  "                               SET RestockCA = 0 " +
  "                               WHERE Asin = '" + obj.Asin + "'; "; 
                     }
                 }
              }
 
            int res = _unitOfWork.asinToSku.updateRestockStatus(sql);
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
