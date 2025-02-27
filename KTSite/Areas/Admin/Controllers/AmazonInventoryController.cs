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
        public IActionResult Index()
        {
          List<AmazonInvStatistics> invList = 
                  _unitOfWork.amazonInventories.GetInventoryStat("US");
          List<SkuQtyForAverage> skuQtyForAverage = 
           _unitOfWork.amazonOrders.GetAllOrdersForAvg("US");
         //List<SkuQtyForAverage> websiteOrdersForAverage = 
           //_unitOfWork.amazonOrders.GetAllWebsiteOrdersForAvg();
          List<AsinToSku> lst = _unitOfWork.asinToSku.GetList();
           int count = 1;
            foreach(AmazonInvStatistics obj in invList)
            {
               obj.Id = count;
               count++;
               obj.avg3days = getAvg(3,obj.sku,skuQtyForAverage, false); 
               obj.avg7days = getAvg(7,obj.sku,skuQtyForAverage, false);
               obj.avgMonth = getAvg(30,obj.sku,skuQtyForAverage, false);
               //obj.avg3daysEbay = getAvg(3,obj.sku,websiteOrdersForAverage,true); 
               if(obj.avg3days == 0)
               { 
                obj.daysToOOS = 10000;
                obj.needToOrderFromChina = false;
                //obj.needToOSendFromWarehouse = false;
               }
               else{ 
                   if(obj.avg3days == 0)
                      obj.daysToOOS = 10000;
                   else
                    obj.daysToOOS = (obj.AmzAvailQty+obj.AmzInboundQty + obj.onTheWay) /(obj.avg3days);
                    
                    
                    
                         obj.needToOrderFromChina = needToOrderFromChina(obj,obj.avg3days,obj.onTheWay);
                  // obj.needToOSendFromWarehouse = needToSendFromWarehouse(obj,
                                                            //          (obj.avg3days));
                 }
              //  obj.restockUS = lst.Where(a=> a.Asin == obj.Asin).Select(a=> a.RestockUS)
              //                  .FirstOrDefault();
            }
            return View(invList);
        }
        public IActionResult CAInv()
        {
          List<AmazonInvStatistics> invList = 
                  _unitOfWork.amazonInventories.GetInventoryStat("CA");
          List<SkuQtyForAverage> skuQtyForAverage = 
           _unitOfWork.amazonOrders.GetAllOrdersForAvg("CA");
         //List<SkuQtyForAverage> websiteOrdersForAverage = 
          // _unitOfWork.amazonOrders.GetAllWebsiteOrdersForAvg();
          List<AsinToSku> lst = _unitOfWork.asinToSku.GetList();
           int count = 1;
            foreach(AmazonInvStatistics obj in invList)
            {
               obj.Id = count;
               count++;
               obj.avg3days = getAvg(3,obj.sku,skuQtyForAverage, false); 
               obj.avg7days = getAvg(7,obj.sku,skuQtyForAverage, false);
               obj.avgMonth = getAvg(30,obj.sku,skuQtyForAverage, false);
               
               if((obj.avg3days) == 0)
               { 
                obj.daysToOOS = 10000;
                obj.needToOrderFromChina = false;
               }
               else{ 
                   obj.daysToOOS = (obj.AmzAvailQty+obj.AmzInboundQty + obj.onTheWay) /(obj.avg3days);
                   
                     obj.needToOrderFromChina = needToOrderFromChina(obj,obj.avg3days,obj.onTheWay );
                 }
                obj.restockCA = lst.Where(a=> a.Asin == obj.Asin).Select(a=> a.RestockCA)
                                .FirstOrDefault();
            }
            return View(invList);
        }
        public int getAvg(int days, string sku, List<SkuQtyForAverage> lst, bool isWebsite)
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
            .Where(a => a.Sku == sku && a.PurchaseDate >= startDate && a.PurchaseDate <= endDate
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
        public IActionResult UpdateInvUS( string model, string  arrId, string checkedVal)
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
 
                bool restock = obj.restockUS;
                int index = Array.IndexOf(Ids, obj.Id);
                if ((bool)obj.restockUS != Vals[index])
                 {
                    if(Vals[index])
                     {
                       sql = sql + " UPDATE AsinToSku " +
  "                               SET RestockUS = 1 " +
  "                               WHERE Asin = '" + obj.Asin + "'; "; 
                     }
                     else
                      {
                       sql = sql + " UPDATE AsinToSku " +
  "                               SET RestockUS = 0 " +
  "                               WHERE Asin = '" + obj.Asin + "'; "; 
                     }
                 }
              }
 
            int res = _unitOfWork.asinToSku.updateRestockStatus(sql);
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
