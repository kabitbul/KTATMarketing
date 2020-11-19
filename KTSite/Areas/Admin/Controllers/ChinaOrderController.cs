using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ChinaOrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChinaOrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var chinaOrder = _unitOfWork.ChinaOrder.GetAll();
            ViewBag.getProductName =
               new Func<int, string>(getProductName);
            return View(chinaOrder);
        }
        public string getProductName(int ProductId)
        {
             return _unitOfWork.Product.GetAll().Where(a => a.Id == ProductId).Select(a => a.ProductName).FirstOrDefault();
        }
        public IActionResult AddChinaOrder()
        {
            ChinaOrderVM chinaOrderVM;
            chinaOrderVM = new ChinaOrderVM()
                {
                chinaOrder = new ChinaOrder(),
                    ProductList = _unitOfWork.Product.GetAll().OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.ProductName,
                        Value = i.Id.ToString()
                    })
                };
            ViewBag.ShowMsg = 0;
            ViewBag.failed = false;
            ViewBag.QuantityZero = false;
            return View(chinaOrderVM);
        }
        public IActionResult UpdateChinaOrder(int Id)
        {
            ChinaOrder chinaOrder = _unitOfWork.ChinaOrder.GetAll().Where(a => a.Id == Id).FirstOrDefault();
            ChinaOrderVM chinaOrderVM = new ChinaOrderVM()
                {
                    chinaOrder = chinaOrder,
                    ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).
                     Select(i => new SelectListItem
                     {
                         Text = i.ProductName,
                         Value = i.Id.ToString()
                     })
                };
            if (chinaOrder.IgnoreMissingQuantity)
            {
                ViewBag.Ignore = true;
            }
            else
            {
                ViewBag.Ignore = false;
            }
            if (chinaOrder.ReceivedAll)
            {
                ViewBag.Received = true;
            }
            else
            {
                ViewBag.Received = false;
            }
            ViewBag.QuantityZero = false;
            
            return View(chinaOrderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddChinaOrder(ChinaOrderVM chinaOrderVM)
        {
            ViewBag.QuantityZero = false;
            ChinaOrderVM chinaOrderVM2 = new ChinaOrderVM()
            {
                chinaOrder = new ChinaOrder(),
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).
                Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                })
            };
            if (ModelState.IsValid)
            {
                if(chinaOrderVM.chinaOrder.Quantity <= 0)
                {
                    ViewBag.QuantityZero = true;
                }
                else if (chinaOrderVM.chinaOrder.Id == 0)
                {
                    ViewBag.QuantityZero = false;
                    _unitOfWork.ChinaOrder.Add(chinaOrderVM.chinaOrder);
                    //Once added, we need to add to the onthe way column on product
                    Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == chinaOrderVM.chinaOrder.ProductId).FirstOrDefault();
                    product.OnTheWayInventory = product.OnTheWayInventory + chinaOrderVM.chinaOrder.Quantity;
                    _unitOfWork.Save();
                }
                ViewBag.ShowMsg = 1;

                //return RedirectToAction(nameof(Index));
            }

           return View(chinaOrderVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateChinaOrder(ChinaOrderVM chinaOrderVM)
        {
            ChinaOrderVM chinaOrderVM2 = new ChinaOrderVM()
            {
                chinaOrder = _unitOfWork.ChinaOrder.GetAll().Where(a => a.Id == chinaOrderVM.chinaOrder.Id).FirstOrDefault(),
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.ProductName,
                        Value = i.Id.ToString()
                    })
            };
            ViewBag.QuantityZero = false;
            if (ModelState.IsValid)
            {
                if (chinaOrderVM.chinaOrder.QuantityReceived <= 0)
                {
                    ViewBag.QuantityZero = true;
                }
                else
                {
                    ChinaOrder oldChinaOrder = _unitOfWork.ChinaOrder.GetAll().Where(a => a.Id == chinaOrderVM.chinaOrder.Id).FirstOrDefault();
                    Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == chinaOrderVM.chinaOrder.ProductId).FirstOrDefault();
                    int QuantityUpdate = chinaOrderVM.chinaOrder.QuantityReceived - oldChinaOrder.QuantityReceived;
                    //update Inventory
                    product.InventoryCount = product.InventoryCount + QuantityUpdate;
                    //update onTheWayInventory
                    if (QuantityUpdate == 0 && !chinaOrderVM.chinaOrder.IgnoreMissingQuantity)//ignore didnt change and quantity didnt change
                    {

                    }
                    else if (QuantityUpdate == 0 && chinaOrderVM.chinaOrder.IgnoreMissingQuantity && !oldChinaOrder.IgnoreMissingQuantity)//quantity didnt change ignore did
                    {
                        int missingQ = chinaOrderVM.chinaOrder.Quantity - chinaOrderVM.chinaOrder.QuantityReceived;
                        if (missingQ >= 0)
                        {
                            product.OnTheWayInventory = product.OnTheWayInventory - missingQ;
                        }
                    }
                    else if (QuantityUpdate != 0 && !chinaOrderVM.chinaOrder.IgnoreMissingQuantity)//quantity changed and ignore didnt
                    {
                        if (chinaOrderVM.chinaOrder.QuantityReceived > chinaOrderVM.chinaOrder.Quantity)
                        {
                            product.OnTheWayInventory = product.OnTheWayInventory - QuantityUpdate + (chinaOrderVM.chinaOrder.QuantityReceived - chinaOrderVM.chinaOrder.Quantity);
                        }
                        else
                        {
                            product.OnTheWayInventory = product.OnTheWayInventory - QuantityUpdate;
                        }
                    }
                    else if (QuantityUpdate != 0 && chinaOrderVM.chinaOrder.IgnoreMissingQuantity && !oldChinaOrder.IgnoreMissingQuantity)//quantity changed and ignore changed
                    {
                        if (chinaOrderVM.chinaOrder.QuantityReceived > chinaOrderVM.chinaOrder.Quantity)
                        {
                            product.OnTheWayInventory = product.OnTheWayInventory - QuantityUpdate + (chinaOrderVM.chinaOrder.QuantityReceived - chinaOrderVM.chinaOrder.Quantity);
                        }
                        else
                        {
                            product.OnTheWayInventory = product.OnTheWayInventory - QuantityUpdate - (chinaOrderVM.chinaOrder.Quantity - chinaOrderVM.chinaOrder.QuantityReceived);
                        }
                    }
                    _unitOfWork.ChinaOrder.update(chinaOrderVM.chinaOrder);
                    _unitOfWork.Save();
                    ViewBag.ShowMsg = 1;
                }
            }
            ViewBag.ShowMsg = 1;
            return View(chinaOrderVM2);
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includePoperties:"Category");
            return Json(new { data = allObj });
        }
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
