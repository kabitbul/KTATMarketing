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
    public class AsinToSkuController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AsinToSkuController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
          List<AsinToSku> lst = _unitOfWork.asinToSku.GetList();
          
            return View(lst);
        }
   public IActionResult Upsert(int id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            AsinToSkuVM asinToSkuVM = new AsinToSkuVM()
            {
                AsinToSku = new AsinToSku(),
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
            asinToSkuVM.AsinToSku = _unitOfWork.asinToSku.GetById(id);
            if (asinToSkuVM.AsinToSku == null)
            {
                return NotFound();
            }
            return View(asinToSkuVM);
        }
        //public IActionResult AddAsinToSku()
        //{
        //  AsinToSkuVM asinToSkuVM = new AsinToSkuVM()
        //    {
        //        AsinToSku = new AsinToSku(),
        //        ProductList = _unitOfWork.Product.GetAll().
        //        OrderBy(a => a.ProductName).Select(i => new SelectListItem
        //        {
        //            Text = i.ProductName,
        //            Value = i.ProductName
        //        })
        //    };
        //    ViewBag.ShowMsg = false;
        //    ViewBag.failed = false;
        //    ViewBag.success = true;
        //    return View(asinToSkuVM);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(AsinToSkuVM asinToSkuVM)
        {
            AsinToSkuVM asinToSkuVM2 = new AsinToSkuVM()
            {
                AsinToSku = new AsinToSku(),
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


                   bool res = _unitOfWork.asinToSku.InsertAsinToSku(asinToSkuVM.AsinToSku.Asin,
                                                      asinToSkuVM.AsinToSku.Sku,
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
                        AsinToSku objFromDb = _unitOfWork.asinToSku.GetById(asinToSkuVM.AsinToSku.Id);
                        asinToSkuVM.AsinToSku.ImageUrl = objFromDb.ImageUrl;
                    }
                }///end image url
                 int res = _unitOfWork.asinToSku.updateById(asinToSkuVM.AsinToSku.Id,
                                                             asinToSkuVM.AsinToSku.Asin,
                                                             asinToSkuVM.AsinToSku.Sku,
                                                             asinToSkuVM.AsinToSku.ChinaName,
                                                              asinToSkuVM.AsinToSku.ImageUrl);
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
         
       //[HttpPost]
       // [ValidateAntiForgeryToken]
       // public IActionResult AddAsinToSku(AsinToSkuVM asinToSkuVM)
       // {
       //     AsinToSkuVM asinToSkuVM2 = new AsinToSkuVM()
       //     {
       //         AsinToSku = new AsinToSku(),
       //         ProductList =  _unitOfWork.Product.GetAll().
       //         OrderBy(a => a.ProductName).Select(i => new SelectListItem
       //         {
       //             Text = i.ProductName,
       //             Value = i.ProductName
       //         })
       //     };
                         
       //     if (ModelState.IsValid)
       //     {
       //        bool res = _unitOfWork.asinToSku.InsertAsinToSku(asinToSkuVM.AsinToSku.Asin,
       //                                               asinToSkuVM.AsinToSku.Sku,
       //                                               asinToSkuVM.AsinToSku.ChinaName);
       //        if(res)
       //         { 
       //           ViewBag.success = true;    
       //           ViewBag.failed = false;
       //           ViewBag.ShowMsg = true;
       //         }
       //         else
       //         {
       //           ViewBag.success = false;    
       //           ViewBag.failed = true;
       //           ViewBag.ShowMsg = true;
       //         }
       //      }
       //      else
       //      {
       //       ViewBag.success = false;    
       //         ViewBag.failed = true;
       //        ViewBag.ShowMsg = false;
       //     }
           
       //     return View(asinToSkuVM2);
       // }
       
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
