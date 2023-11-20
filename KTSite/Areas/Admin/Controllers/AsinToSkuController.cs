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

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AsinToSkuController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AsinToSkuController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
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
                   bool res = _unitOfWork.asinToSku.InsertAsinToSku(asinToSkuVM.AsinToSku.Asin,
                                                      asinToSkuVM.AsinToSku.Sku,
                                                      asinToSkuVM.AsinToSku.ChinaName);
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
                 int res = _unitOfWork.asinToSku.updateById(asinToSkuVM.AsinToSku.Id,
                                                             asinToSkuVM.AsinToSku.Asin,
                                                             asinToSkuVM.AsinToSku.Sku,
                                                             asinToSkuVM.AsinToSku.ChinaName);
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
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll();
            return Json(new { data = allObj });
        }

        #endregion
    }
}
