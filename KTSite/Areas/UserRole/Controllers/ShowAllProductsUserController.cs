using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [AllowAnonymous]
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
            uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            var productsList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers && ((a.MerchId != SD.Kfir_Merch && uNameId != SD.Kfir_Buyer)||
                                                                                               (uNameId == SD.Kfir_Buyer)))
            .OrderBy(a=>a.ProductName);
            

            dynamic myModel = new System.Dynamic.ExpandoObject();
            myModel.Products = productsList;
            myModel.SellInv = _unitOfWork.SellersInventory.GetAll().Where(a => a.UserNameId == uNameId);
            ViewBag.uNameID = uNameId;
            ViewBag.uName = uName;
            ViewBag.getStoreName =
               new Func<int,string>(returnStoreName);
            ViewBag.getCategory =
                  new Func<int, string>(getCategory);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.getSellerCost =
               new Func<double, string>(returnSellerCost);
            return View(myModel);
        }
        public string getCategory(int Id)
        {
            return _unitOfWork.Category.GetAll().Where(a => a.Id == Id).Select(a => a.Name).FirstOrDefault();
        }
        public string returnSellerCost(double price)
        {
            string uNameId = returnUserNameId();
            if(uNameId == SD.FBMP_USER_KARIN)
            {
                return (price + 1).ToString("0.00");
            }
            return price.ToString("0.00");
        }
        public IActionResult AddStoreToProduct(int Id)
        {
            string uNameId = "";
            string uName = "";
            uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            SellersInventoryVM sellersInventoryVM = new SellersInventoryVM()
            {
                SellersInventory = new SellersInventory(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers).
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
                }),
                storesChecked = null
            };
            List<int> storeIdWithProduct = _unitOfWork.SellersInventory.GetAll().
                Where(q => q.UserNameId == uNameId && q.ProductId == Id).Select(q => q.StoreNameId).ToList();
            List<StoresChecked> storesChecked = new List<StoresChecked>();
            int i = 0;

            foreach(SelectListItem item in sellersInventoryVM.StoresList)
            {
                i++;
                if(storeIdWithProduct.Contains(Convert.ToInt32(item.Value)))
                   storesChecked.Add(new StoresChecked { StoreName = item.Text, Value = Convert.ToInt32(item.Value),IsChecked = true});
                else
                   storesChecked.Add(new StoresChecked { StoreName = item.Text, Value = Convert.ToInt32(item.Value), IsChecked = false});
            }
            sellersInventoryVM.storesChecked = storesChecked;
            ViewBag.uNameID = uNameId;
            ViewBag.uName = uName;
            ViewBag.getStoreName =
                new Func<int,string>(returnStoreName);
            ViewBag.success = true;
            ViewBag.ShowMsg = false;
            sellersInventoryVM.SellersInventory.ProductId = Id;
            return View(sellersInventoryVM);
        }
        public IActionResult DeleteStoreToProduct()
        {
            string userNameId = returnUserNameId();
            ViewBag.getProductName =
               new Func<int, string>(returnProductName);
            ViewBag.getStoreName =
               new Func<int, string>(returnStoreName);
            var sellersInventory = _unitOfWork.SellersInventory.GetAll().Where(a => a.UserNameId == userNameId);
                return View(sellersInventory);
        }
        public string returnProductName(int productId)
        {
            return _unitOfWork.Product.Get(productId).ProductName;
        }
        public string returnStoreName(int storeId)
        {
            return _unitOfWork.UserStoreName.Get(storeId).StoreName;
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
                if (sellersInventoryVM.storesChecked != null)
                {
                    foreach (StoresChecked sto in sellersInventoryVM.storesChecked)
                    {
                        bool existStore = _unitOfWork.SellersInventory.GetAll().
                            Any(a => a.UserNameId == sellersInventoryVM.SellersInventory.UserNameId &&
                                  a.ProductId == sellersInventoryVM.SellersInventory.ProductId &&
                                  a.StoreNameId == sto.Value);
                        if (sto.IsChecked && !existStore)
                        {
                            SellersInventory sv = new SellersInventory();
                            sv.ProductId = sellersInventoryVM.SellersInventory.ProductId;
                            sv.UserNameId = sellersInventoryVM.SellersInventory.UserNameId;
                            sv.UserName = sellersInventoryVM.SellersInventory.UserName;
                            sv.StoreNameId = sto.Value;
                            _unitOfWork.SellersInventory.Add(sv);
                            _unitOfWork.Save();
                        }
                        else if (!sto.IsChecked && existStore)
                        {
                            int Id = _unitOfWork.SellersInventory.GetAll().Where(a => a.ProductId == sellersInventoryVM.SellersInventory.ProductId &&
                             a.UserNameId == sellersInventoryVM.SellersInventory.UserNameId &&
                             a.StoreNameId == sto.Value).Select(a => a.Id).FirstOrDefault();
                            _unitOfWork.SellersInventory.Remove(Id);
                            _unitOfWork.Save();
                        }
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
