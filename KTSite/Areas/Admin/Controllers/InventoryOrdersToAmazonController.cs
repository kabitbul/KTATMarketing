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
    public class InventoryOrdersToAmazonController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryOrdersToAmazonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var inventoryOrd = _unitOfWork.inventoryOrdersToAmazon.GetList();
            //ViewBag.getProductName =
             //  new Func<int, string>(getProductName);
            return View(inventoryOrd);
        }
       // public string getProductName(int ProductId)
        //{
        //     return _unitOfWork.Product.GetAll().Where(a => a.Id == ProductId).Select(a => a.ProductName).FirstOrDefault();
       // }
        public IActionResult AddInventoryOrder()
        {
            InventoryOrdersToAmazonVM inventoryOrdersToAmazonVM;
            inventoryOrdersToAmazonVM = new InventoryOrdersToAmazonVM()
                {
                inventoryOrdersToAmazon = new InventoryOrdersToAmazon(),
                    ProductList = _unitOfWork.asinToSku.GetList(). //GetAll().Where(a=>a.MerchId == null).OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.Sku,
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
            InventoryOrdersToAmazon inventoryOrdersToAmazon = _unitOfWork.inventoryOrdersToAmazon.GetById(Id);
            InventoryOrdersToAmazonVM inventoryOrdersToAmazonVM = new InventoryOrdersToAmazonVM()
                {
                    inventoryOrdersToAmazon = inventoryOrdersToAmazon,
                    ProductList = _unitOfWork.asinToSku.GetList(). //GetAll().Where(a=>a.MerchId == null).OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.Sku,
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
        public IActionResult AddInventoryOrder(InventoryOrdersToAmazonVM inventoryOrdersToAmazonVM)
        {
            ViewBag.QuantityZero = false;
            ViewBag.success = false;
            InventoryOrdersToAmazonVM inventoryOrdersToAmazonVM2 = new InventoryOrdersToAmazonVM()
            {
                inventoryOrdersToAmazon = new InventoryOrdersToAmazon(),
                ProductList = _unitOfWork.asinToSku.GetList(). //GetAll().Where(a=>a.MerchId == null).OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.Sku,
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
                    inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.ProductSku = _unitOfWork.asinToSku.GetSkuByAsin(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.ProductAsin);
                    _unitOfWork.inventoryOrdersToAmazon.InsertInvOrder(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.ProductAsin,
                                                                       inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.ProductSku,
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
        public IActionResult UpdateInventoryOrder(InventoryOrdersToAmazonVM inventoryOrdersToAmazonVM)
        {
            string uNameId = returnUserNameId();
            ViewBag.success = false;
            InventoryOrdersToAmazonVM inventoryOrdersToAmazonVM2 = new InventoryOrdersToAmazonVM()
            {
                inventoryOrdersToAmazon = _unitOfWork.inventoryOrdersToAmazon.GetById(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Id),
                ProductList = _unitOfWork.asinToSku.GetList(). //GetAll().Where(a=>a.MerchId == null).OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.Asin + " - " + i.Sku,
                        Value = i.Asin
                    })
            };
            ViewBag.QuantityZero = false;
            if (ModelState.IsValid)
            {
                
               int res =     _unitOfWork.inventoryOrdersToAmazon.updateById(inventoryOrdersToAmazonVM.inventoryOrdersToAmazon.Id,
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
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
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
