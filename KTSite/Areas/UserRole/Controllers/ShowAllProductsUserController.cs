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

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [Authorize(Roles = SD.Role_Users)]
    public class ShowAllProductsUserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ShowAllProductsUserController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            string uNameId = "";
            string uName = "";
            var productsList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers).OrderBy(a=>a.ProductName);
            uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();

            dynamic myModel = new System.Dynamic.ExpandoObject();
            myModel.Products = productsList;
            myModel.SellInv = _unitOfWork.SellersInventory.GetAll().Where(a => a.UserNameId == uNameId);
            ViewBag.uNameID = uNameId;
            ViewBag.uName = uName;
            ViewBag.getStoreName =
               new Func<int,string>(returnStoreName);
            return View(myModel);
        }
        public IActionResult AddStoreToProduct()
        {
            string uNameId = "";
            string uName = "";
            uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            SellersInventoryVM sellersInventoryVM = new SellersInventoryVM()
            {
                SellersInventory = new SellersInventory(),
                ProductList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers).
                OrderBy(a => a.ProductName).
                Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.UserNameId == uNameId).Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                })
            };
            ViewBag.uNameID = uNameId;
            ViewBag.uName = uName;
            ViewBag.getStoreName =
                new Func<int,string>(returnStoreName);
            ViewBag.success = true;
            ViewBag.ShowMsg = false;
            return View(sellersInventoryVM);
        }
        public IActionResult DeleteStoreToProduct()
        {
            ViewBag.getProductName =
               new Func<int, string>(returnProductName);
            ViewBag.getStoreName =
               new Func<int, string>(returnStoreName);
            var sellersInventory = _unitOfWork.SellersInventory.GetAll().Where(a => a.UserNameId == returnUserNameId());
                return View(sellersInventory);
        }
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }
        public string returnStoreName(int storeId)
        {
            return (_unitOfWork.UserStoreName.GetAll().Where(q => q.Id == storeId).Select(q => q.StoreName)).FirstOrDefault();
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStoreToProduct(SellersInventoryVM sellersInventoryVM)
        {
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {
                if(sellersInventoryVM.updateAllStores)
                {
                    string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
                    //get all stores of user
                    var StoresIdList = _unitOfWork.UserStoreName.GetAll().Where(q => q.UserNameId == uNameId).Select(i => i.Id);
                    foreach(var storeId in StoresIdList)
                    {
                        if (_unitOfWork.SellersInventory.GetAll().Where(q => q.ProductId == sellersInventoryVM.SellersInventory.ProductId).
                             Where(a => a.StoreNameId == storeId).Count() == 0)
                        {
                            sellersInventoryVM.SellersInventory.Id = 0;
                            sellersInventoryVM.SellersInventory.StoreNameId = storeId;
                            _unitOfWork.SellersInventory.Add(sellersInventoryVM.SellersInventory);
                            _unitOfWork.Save();
                        }
                    }
                    
                }
                else
                {
                    bool existStore =
                        _unitOfWork.SellersInventory.GetAll().Any(a => a.UserNameId == returnUserNameId() &&
                                               a.StoreNameId == sellersInventoryVM.SellersInventory.StoreNameId &&
                                               a.ProductId == sellersInventoryVM.SellersInventory.ProductId);
                    if (!existStore)
                    {
                        _unitOfWork.SellersInventory.Add(sellersInventoryVM.SellersInventory);
                        _unitOfWork.Save();
                    }
                }

                ViewBag.success = true;

                return RedirectToAction(nameof(Index));
            }
            return View(sellersInventoryVM.SellersInventory);
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
            var objFromDb = _unitOfWork.SellersInventory.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.SellersInventory.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
