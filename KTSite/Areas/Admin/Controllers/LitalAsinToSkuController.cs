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
    public class LitalAsinToSkuController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public LitalAsinToSkuController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
          List<LitalAsinToSku> lst = _unitOfWork.litalAsinToSku.GetList();
          
            return View(lst);
        }
   public IActionResult Upsert(int id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            LitalAsinToSkuVM asinToSkuVM = new LitalAsinToSkuVM()
            {
                AsinToSku = new LitalAsinToSku(),
                ProductList = _unitOfWork.Product.GetAll().
                OrderBy(a => a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.ProductName
                })
            };
            if(id == 0)//create
            {
                return View(asinToSkuVM);
            }
            asinToSkuVM.AsinToSku = _unitOfWork.litalAsinToSku.GetById(id);
                if (asinToSkuVM.AsinToSku == null)
            {
                return NotFound();
            }
            return View(asinToSkuVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(LitalAsinToSkuVM asinToSkuVM)
        {
            LitalAsinToSkuVM asinToSkuVM2 = new LitalAsinToSkuVM()
            {
                AsinToSku = new LitalAsinToSku(),
                ProductList =  _unitOfWork.Product.GetAll().
                OrderBy(a => a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.ProductName
                })
            };
                         
            if (ModelState.IsValid)
            {
               if( asinToSkuVM.AsinToSku.Id == 0)
                { 
string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"Images\Products");
                    var extention = Path.GetExtension(files[0].FileName);
                    if (asinToSkuVM.AsinToSku.ImageUrl != null)
                    {
                        //this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, asinToSkuVM.AsinToSku.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var filesStreams = new FileStream(Path.Combine(uploads,fileName+extention),FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    asinToSkuVM.AsinToSku.ImageUrl = @"\Images\Products\" + fileName + extention;
                }
                else
                {
                    //update when they do not change the image
                    if( asinToSkuVM.AsinToSku.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(asinToSkuVM.AsinToSku.Id);
                        asinToSkuVM.AsinToSku.ImageUrl = objFromDb.ImageUrl;
                    }
                }///end image url


                   bool res = _unitOfWork.litalAsinToSku.InsertAsinToSku(asinToSkuVM.AsinToSku.Asin,
                                                                         asinToSkuVM.AsinToSku.ChinaName,
                                                                         asinToSkuVM.AsinToSku.ImageUrl);
               if(res)
                { 
                  ViewBag.success = true;    
                  ViewBag.failed = false;
                  ViewBag.ShowMsg = true;
                }
                else
                {
                  ViewBag.success = false;    
                  ViewBag.failed = true;
                  ViewBag.ShowMsg = true;
                }
               }
              else
              {
               string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"Images\Products");
                    var extention = Path.GetExtension(files[0].FileName);
                    if (asinToSkuVM.AsinToSku.ImageUrl != null)
                    {
                        //this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, asinToSkuVM.AsinToSku.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var filesStreams = new FileStream(Path.Combine(uploads,fileName+extention),FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    asinToSkuVM.AsinToSku.ImageUrl = @"\Images\Products\" + fileName + extention;
                }
                else
                {
                    //update when they do not change the image
                    if( asinToSkuVM.AsinToSku.Id != 0)
                    {
                        LitalAsinToSku objFromDb = _unitOfWork.litalAsinToSku.GetById(asinToSkuVM.AsinToSku.Id);
                        asinToSkuVM.AsinToSku.ImageUrl = objFromDb.ImageUrl;
                    }
                }///end image url
                 int res = _unitOfWork.litalAsinToSku.updateById(asinToSkuVM.AsinToSku.Id,
                                                             asinToSkuVM.AsinToSku.Asin,
                                                             asinToSkuVM.AsinToSku.ChinaName,
                                                             asinToSkuVM.AsinToSku.ImageUrl,
                                                             asinToSkuVM.AsinToSku.RestockNOTDECIDED);
               if(res == 1)
                { 
                  ViewBag.success = true;    
                  ViewBag.failed = false;
                  ViewBag.ShowMsg = true;
                }
                else
                {
                  ViewBag.success = false;    
                  ViewBag.failed = true;
                  ViewBag.ShowMsg = true;
                } 
               }
             }
             else
             {
              ViewBag.success = false;    
                ViewBag.failed = true;
               ViewBag.ShowMsg = false;
            }
           
            return View(asinToSkuVM2);
        }
         public IActionResult GraphUS(int Id)
        {
            //stack chart
            List<GraphData> graphData = new List<GraphData>();
            LitalAsinToSku asinToSku = _unitOfWork.litalAsinToSku.GetById(Id);
                if(asinToSku == null)
                  return View();
            
            graphData = _unitOfWork.litalAmazonOrders.GetGraphData("US",asinToSku.Asin);
            ViewBag.ProductName = asinToSku.ChinaName;
            
            ViewBag.DataPointsG = JsonConvert.SerializeObject(graphData);
            
            return View();
        }
 public IActionResult GraphTotalUS()
        {
            //stack chart
            List<GraphData> graphData = new List<GraphData>();
            
            graphData = _unitOfWork.litalAmazonOrders.GetTotalOrdGraphData("US");
            
            ViewBag.DataPointsG = JsonConvert.SerializeObject(graphData);
            
            return View();
        }
        #region API CALLS
[HttpDelete]
        public IActionResult Delete(int id)
        {
            bool effected =  _unitOfWork.asinToSku.DeleteById(id);
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
