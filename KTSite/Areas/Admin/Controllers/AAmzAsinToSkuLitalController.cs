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
using Microsoft.AspNetCore.Mvc.Rendering;
using MailKit.Search;
using System.IO;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmzAsinToSkuLitalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        int gVarStoreId = SD.LitalStoreId;//---------GLOBAL CONST
        
        public AAmzAsinToSkuLitalController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
          List<AAmzAsinToSku> lst = _unitOfWork.AAmzAsinToSku.GetList(gVarStoreId);
          
            return View(lst);
        }
   public IActionResult Upsert(int id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            AAmzAsinToSku aAmzAsinToSku = _unitOfWork.AAmzAsinToSku.getForUpsert(id,gVarStoreId);
            if (aAmzAsinToSku == null)
            {
                return NotFound();
            }
            return View(aAmzAsinToSku);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(AAmzAsinToSku aAmzAsinToSku)
        {
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {
                bool res = _unitOfWork.AAmzAsinToSku.Upsert(aAmzAsinToSku,gVarStoreId);
                if(res){ 
                  ViewBag.failed = false;
                  return RedirectToAction(nameof(Index));
                 }
                else{ 
                  ViewBag.failed = true;
                  return View(aAmzAsinToSku);
                 }
            }
            ViewBag.failed = true;
            return View(aAmzAsinToSku);
        }
         public IActionResult GraphUS(int Id)
        {
            //stack chart
            List<GraphData> graphData = new List<GraphData>();
            AAmzAsinToSku asinToSku = _unitOfWork.AAmzAsinToSku.GetById(Id,gVarStoreId);
                if(asinToSku == null)
                  return View();
            
            graphData = _unitOfWork.AAmzOrders.GetGraphData(gVarStoreId, SD.marketPlaceUS,asinToSku.Asin);
            ViewBag.ProductName = asinToSku.ChinaName;
            
            ViewBag.DataPointsG = JsonConvert.SerializeObject(graphData);
            
            return View();
        }
      public IActionResult GraphTotalUS()
        {
            //stack chart
            List<GraphData> graphData = new List<GraphData>();
            
            
            graphData = _unitOfWork.AAmzOrders.GetTotalOrdGraphData(SD.marketPlaceUS,gVarStoreId);
            
            ViewBag.DataPointsG = JsonConvert.SerializeObject(graphData);
            
            return View();
        }
         public IActionResult GraphCA(int Id)
        {
            //stack chart
            List<GraphData> graphData = new List<GraphData>();
            AAmzAsinToSku asinToSku = _unitOfWork.AAmzAsinToSku.GetById(Id,gVarStoreId);
                if(asinToSku == null)
                  return View();
            
            graphData = _unitOfWork.AAmzOrders.GetGraphData(gVarStoreId,SD.marketPlaceCA, asinToSku.Asin);
            ViewBag.ProductName = asinToSku.ChinaName;
            
            ViewBag.DataPointsG = JsonConvert.SerializeObject(graphData);
            return View();
        }
      public IActionResult GraphTotalCA()
        {
            //stack chart
            List<GraphData> graphData = new List<GraphData>();
            
            graphData = _unitOfWork.AAmzOrders.GetTotalOrdGraphData(SD.marketPlaceCA,gVarStoreId);
            
            ViewBag.DataPointsG = JsonConvert.SerializeObject(graphData);
            
            return View();
        }
       
        #region API CALLS
[HttpDelete]
        public IActionResult Delete(int id)
        {
            bool effected =  _unitOfWork.AAmzAsinToSku.DeleteById(id,SD.KTStoreId);
            if(!effected)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
