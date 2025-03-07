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
    public class LitalInventoryOrdersToAmazonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public LitalInventoryOrdersToAmazonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var inventoryOrd = _unitOfWork.litalInventoryOrdersToAmazon.GetList();
            
            return View(inventoryOrd);
        }

        public IActionResult AddInventoryOrder()
        {
            LitalInventoryOrdersToAmazonVM inventoryOrdersToAmazonVM;
            inventoryOrdersToAmazonVM = new LitalInventoryOrdersToAmazonVM()
                {
                inventoryOrdersToAmazon = new LitalInventoryOrdersToAmazon(),
                    ProductList = _unitOfWork.asinToSku.GetList(). //GetAll().Where(a=>a.MerchId == null).OrderBy(a=>a.ProductName).
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
            return View(inventoryOrdersToAmazonVM);
        }
        public IActionResult UpdateInventoryOrder(int Id)
        {
            LitalInventoryOrdersToAmazon inventoryOrdersToAmazon = _unitOfWork.litalInventoryOrdersToAmazon.GetById(Id);
            LitalInventoryOrdersToAmazonVM inventoryOrdersToAmazonVM = new LitalInventoryOrdersToAmazonVM()
                {
                    inventoryOrdersToAmazon = inventoryOrdersToAmazon,
                    ProductList = _unitOfWork.litalAsinToSku.GetList().
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
                };
           
            if (inventoryOrdersToAmazon.InboundUpdated)
            {
                ViewBag.Received = true;
            }
            else
            {
                ViewBag.Received = false;
            }
            ViewBag.QuantityZero = false;
            ViewBag.success = true;
            return View(inventoryOrdersToAmazonVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInventoryOrder(LitalInventoryOrdersToAmazonVM inventoryOrdersToAmazonVM)
        {
            ViewBag.QuantityZero = false;
            ViewBag.success = false;
            LitalInventoryOrdersToAmazonVM inventoryOrdersToAmazonVM2 = new LitalInventoryOrdersToAmazonVM()
            {
                inventoryOrdersToAmazon = new LitalInventoryOrdersToAmazon(),
                ProductList = _unitOfWork.litalAsinToSku.GetList().
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
            };
            if (ModelState.IsValid)
            {
                if(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Quantity <= 0)
                {
                    ViewBag.QuantityZero = true;
                }
                else if (inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Id == 0)
                {
                    ViewBag.QuantityZero = false;
                    _unitOfWork.litalInventoryOrdersToAmazon.InsertInvOrder(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.ProductAsin,
                                                                       inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Quantity,
                                                                        inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.DateOrdered);
                    
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
                ViewBag.ShowMsg = 1;

                //return RedirectToAction(nameof(Index));
            }

           return View(inventoryOrdersToAmazonVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInventoryOrder(LitalInventoryOrdersToAmazonVM inventoryOrdersToAmazonVM)
        {
            ViewBag.success = false;
            LitalInventoryOrdersToAmazonVM inventoryOrdersToAmazonVM2 = new LitalInventoryOrdersToAmazonVM()
            {
                inventoryOrdersToAmazon = _unitOfWork.litalInventoryOrdersToAmazon.GetById(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Id),
                ProductList = _unitOfWork.asinToSku.GetList(). //GetAll().Where(a=>a.MerchId == null).OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
            };
            ViewBag.QuantityZero = false;
            if (ModelState.IsValid)
            {
                
               int res =     _unitOfWork.litalInventoryOrdersToAmazon.updateById(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Id,
                                                                     inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Quantity,
                                                                      inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.DateOrdered,
                                                                        inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.InboundUpdated); 
              if (res == 0)
                 ViewBag.success = false;
             else
             { 
                    _unitOfWork.Save();
                    ViewBag.success = true;
                    ViewBag.ShowMsg = 1;
                }
            }
            ViewBag.ShowMsg = 1;
            return View(inventoryOrdersToAmazonVM2);
        }
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
