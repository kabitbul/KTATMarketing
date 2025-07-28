using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmzStockPurchaseLitalUSController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        int gVarStoreId = SD.LitalStoreId;//---------GLOBAL CONST
        string gVarMarketplace = SD.marketPlaceUS;//--------GLOBAL CONST
        public AAmzStockPurchaseLitalUSController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var inventoryOrd = _unitOfWork.AAmzStockPurchase.GetList(gVarStoreId,gVarMarketplace);
            return View(inventoryOrd);
        }

        public IActionResult AddStock()
        {
            AAmzStockPurchaseVM aAmzStockPurchaseVM;
            aAmzStockPurchaseVM = new AAmzStockPurchaseVM()
                {
                aAmzStockPurchase = new AAmzStockPurchase(),
                    ProductList = _unitOfWork.AAmzAsinToSku.GetListByMarketplace(gVarStoreId,gVarMarketplace). 
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
                };
            ViewBag.ShowMsg = 0;
            ViewBag.failed = false;
            ViewBag.QuantityZero = false;
            ViewBag.success = true;
            return View(aAmzStockPurchaseVM);
        }
        public IActionResult UpdateStock(int Id)
        {
            AAmzStockPurchase aAmzStockPurchase = _unitOfWork.AAmzStockPurchase.GetById(Id);
            AAmzStockPurchaseVM aAmzStockPurchaseVM = new AAmzStockPurchaseVM()
                {
                    aAmzStockPurchase = aAmzStockPurchase,
                    ProductList = _unitOfWork.AAmzAsinToSku.GetListByMarketplace(gVarStoreId,gVarMarketplace). 
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
                };
           
            if (aAmzStockPurchase.InboundUpdated)
            {
                ViewBag.Received = true;
            }
            else
            {
                ViewBag.Received = false;
            }
            ViewBag.QuantityZero = false;
            ViewBag.success = true;
            return View(aAmzStockPurchaseVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStock(AAmzStockPurchaseVM aAmzStockPurchaseVM)
        {
            ViewBag.QuantityZero = false;
            ViewBag.success = false;
            AAmzStockPurchaseVM aAmzStockPurchaseVM2 = new AAmzStockPurchaseVM()
            {
                aAmzStockPurchase = new AAmzStockPurchase(),
                ProductList = _unitOfWork.AAmzAsinToSku.GetListByMarketplace(gVarStoreId,gVarMarketplace). 
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
            };
            if (ModelState.IsValid)
            {
                if(aAmzStockPurchaseVM.aAmzStockPurchase.Quantity <= 0)
                {
                    ViewBag.QuantityZero = true;
                }
                else if (aAmzStockPurchaseVM.aAmzStockPurchase.Id == 0)
                {
                    ViewBag.QuantityZero = false;
              aAmzStockPurchaseVM.aAmzStockPurchase.ProductChinaName =       
              _unitOfWork.AAmzAsinToSku.GetSkuByAsin(aAmzStockPurchaseVM.aAmzStockPurchase.ProductAsin);
                   bool res = _unitOfWork.AAmzStockPurchase.AddStock(aAmzStockPurchaseVM) ;                  
                   if(res)
                    ViewBag.success = true;
                }
                ViewBag.ShowMsg = 1; 
            }
           return View(aAmzStockPurchaseVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStock(AAmzStockPurchaseVM aAmzStockPurchaseVM)
        {
            ViewBag.success = false;
            AAmzStockPurchaseVM aAmzStockPurchaseVM2 = new AAmzStockPurchaseVM()
            {
                aAmzStockPurchase = new AAmzStockPurchase(),
                ProductList = _unitOfWork.AAmzAsinToSku.GetListByMarketplace(gVarStoreId,gVarMarketplace). 
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
            };
            ViewBag.QuantityZero = false;
            if (ModelState.IsValid)
            {
                
               int res =     _unitOfWork.AAmzStockPurchase.updateById(aAmzStockPurchaseVM);
              if (res == 0)
                 ViewBag.success = false;
             else
             {     
                    ViewBag.success = true;
                    ViewBag.ShowMsg = 1;
                }
            }
            else{
foreach (var state in ModelState)
{
    var key = state.Key;
    var errors = state.Value.Errors;

    foreach (var error in errors)
    {
        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
    }
}
            }
            ViewBag.ShowMsg = 1;
            return View(aAmzStockPurchaseVM2);
        }
        //public string returnUserNameId()
        //{
        //    return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        //}
        #region API CALLS
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
