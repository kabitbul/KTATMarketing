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
    public class InventoryOrdersToAmzCAController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryOrdersToAmzCAController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var inventoryOrd = _unitOfWork.inventoryOrdersToAmzCA.GetList();
            return View(inventoryOrd);
        }
        public IActionResult AddInventoryOrderCA()
        {
            InventoryOrdersToAmzCAVM inventoryOrdersToAmzCAVM;
            inventoryOrdersToAmzCAVM = new InventoryOrdersToAmzCAVM()
                {
                inventoryOrdersToAmzCA = new InventoryOrdersToAmzCA(),
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
            return View(inventoryOrdersToAmzCAVM);
        }
        public IActionResult UpdateInventoryOrderCA(int Id)
        {
            InventoryOrdersToAmzCA inventoryOrdersToAmzCA = _unitOfWork.inventoryOrdersToAmzCA.GetById(Id);
            InventoryOrdersToAmzCAVM inventoryOrdersToAmzCAVM = new InventoryOrdersToAmzCAVM()
                {
                    inventoryOrdersToAmzCA = inventoryOrdersToAmzCA,
                    ProductList = _unitOfWork.asinToSku.GetList(). //GetAll().Where(a=>a.MerchId == null).OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
                };
           
            if (inventoryOrdersToAmzCA.InboundUpdated)
            {
                ViewBag.Received = true;
            }
            else
            {
                ViewBag.Received = false;
            }
            ViewBag.QuantityZero = false;
            ViewBag.success = true;
            return View(inventoryOrdersToAmzCAVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInventoryOrderCA(InventoryOrdersToAmzCAVM inventoryOrdersToAmzCAVM)
        {
            ViewBag.QuantityZero = false;
            ViewBag.success = false;
            InventoryOrdersToAmzCAVM inventoryOrdersToAmzCAVM2 = new InventoryOrdersToAmzCAVM()
            {
                inventoryOrdersToAmzCA = new InventoryOrdersToAmzCA(),
                ProductList = _unitOfWork.asinToSku.GetList().
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
            };
            if (ModelState.IsValid)
            {
                if(inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.Quantity <= 0)
                {
                    ViewBag.QuantityZero = true;
                }
                else if (inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.Id == 0)
                {
                    ViewBag.QuantityZero = false;
                    inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.ProductSku = _unitOfWork.asinToSku.GetSkuByAsin(inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.ProductAsin);
                    _unitOfWork.inventoryOrdersToAmzCA.InsertInvOrder(inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.ProductAsin,
                                                                      inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.ProductSku,
                                                                      inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.Quantity,
                                                                      inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.DateOrdered);
                    
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
                ViewBag.ShowMsg = 1;

                //return RedirectToAction(nameof(Index));
            }

           return View(inventoryOrdersToAmzCAVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInventoryOrderCA(InventoryOrdersToAmzCAVM inventoryOrdersToAmzCAVM)
        {
            
            ViewBag.success = false;
            InventoryOrdersToAmzCAVM inventoryOrdersToAmzCAVM2 = new InventoryOrdersToAmzCAVM()
            {
                inventoryOrdersToAmzCA = _unitOfWork.inventoryOrdersToAmzCA.GetById(inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.Id),
                ProductList = _unitOfWork.asinToSku.GetList().
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.ChinaName,
                        Value = i.Asin
                    })
            };
            ViewBag.QuantityZero = false;
            if (ModelState.IsValid)
            {
                
               int res =     _unitOfWork.inventoryOrdersToAmazon.updateById(inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.Id,
                                                                     inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.Quantity,
                                                                      inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.DateOrdered,
                                                                        inventoryOrdersToAmzCAVM.inventoryOrdersToAmzCA.InboundUpdated); 
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
            return View(inventoryOrdersToAmzCAVM2);
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
