using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            string userNameId = returnUserNameId();
            IEnumerable<Product> product = _unitOfWork.Product.GetAll().Where(a=> a.MerchId == userNameId).OrderBy(a => a.ProductName);
            ViewBag.userNameId = userNameId;
            foreach (Product prod in product)
            {
                //if (!prod.OwnByWarehouse)
                //{
                //    //prod.Cost = 0;
                //    prod.WarehouseProfit = 0;
                //}
                //else
                //{
                //    //prod.WarehouseProfit = prod
                //}
            }
            ViewBag.getCategoryName =
              new Func<int, string>(getCategoryName);
            ViewBag.Profit =
              new Func<int, string>(getProfit);
            ViewBag.ShippingCost =
              new Func<int, string>(getShippingCost);
            return View(product);
        }
        public string getProfit(int productId)
        {
           double dollar;
            double addToMinimum;
           Product product = _unitOfWork.Product.Get(productId);
           dollar = product.Cost - (SD.FeesOfKTMerch * product.Cost) - (product.ShippingCharge+SD.addToKTMerchRate);
            if ((product.SellersCost * SD.FeesOfKTMerch) < 0.8)
            {
                addToMinimum = 0.8 - (product.SellersCost * SD.FeesOfKTMerch);
                dollar = dollar - (addToMinimum);
            }

            return dollar.ToString("0.00") + "$";
        }
        public string getShippingCost(int productId)
        {
            double dollar;
            Product product = _unitOfWork.Product.Get(productId);
            dollar = (product.ShippingCharge + SD.addToKTMerchRate);

            return dollar.ToString("0.00") + "$";
        }
        public string getCategoryName(int CategoryId)
        {
            return _unitOfWork.Category.GetAll().Where(a => a.Id == CategoryId).Select(a => a.Name).FirstOrDefault();
        }
        public IActionResult Upsert(int? id)
        {
            ViewBag.existProd = false;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            string uNameId = returnUserNameId();
            ViewBag.userNameId = uNameId;
            ProductVM productVM = new ProductVM()
            {

                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                MadeInList = SD.MadeInState
            };
            if(id == null)//create
            {
                ViewBag.create = true;
                productVM.Product.ReStock = true;
                return View(productVM);
            }
            ViewBag.create = false;
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
            ProductVM productVM2 = new ProductVM()
            {

                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                MadeInList = SD.MadeInState
            };
            if (ModelState.IsValid)
            {
                productVM.Product.Cost = productVM.Product.SellersCost;
                ViewBag.userNameId = returnUserNameId();
                if (!productVM.Product.OwnByWarehouse)
                {
                    productVM.Product.WarehouseChinaCost = 0;
                }
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"Images\Products");
                    var extention = Path.GetExtension(files[0].FileName);
                    if (productVM.Product.ImageUrl != null)
                    {
                        //this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var filesStreams = new FileStream(Path.Combine(uploads,fileName+extention),FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productVM.Product.ImageUrl = @"\Images\Products\" + fileName + extention;
                }
                else
                {
                    //update when they do not change the image
                    if( productVM.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }
                if (productVM.Product.Id == 0)
                {
                    bool existProd =_unitOfWork.Product.GetAll().Any(a => a.ProductName.ToLower() == productVM.Product.ProductName.ToLower());
                    if (existProd)
                    {
                        ViewBag.ShowMsg = true;
                        ViewBag.existProd = true;
                        return View(productVM2);
                    }
                    else
                    {
                        if (productVM.Product.Weight <= 0 || productVM.Product.Weight >= 1)
                        {
                            ViewBag.ShowMsg = true;
                            ViewBag.invalidWeight = true;
                            ViewBag.existProd = false;
                            return View(productVM2);
                        }
                        else
                        {
                            ViewBag.invalidWeight = false;
                            if (productVM.Product.Weight < SD.weightFor1Rate && productVM.Product.Weight > 0)
                            {
                                productVM.Product.ShippingCharge = SD.priceFor1Rate;
                            }
                            else if (productVM.Product.Weight < SD.weightFor2Rate && productVM.Product.Weight >= SD.weightFor1Rate)
                            {
                                productVM.Product.ShippingCharge = SD.priceFor2Rate;
                            }
                            else if (productVM.Product.Weight < SD.weightForMaxRate && productVM.Product.Weight >= SD.weightFor2Rate)
                            {
                                productVM.Product.ShippingCharge = SD.priceForMaxRate;
                            }
                        }
                        if (productVM.Product.SellersCost <= 0)
                        {
                            ViewBag.ShowMsg = true;
                            ViewBag.invalidPrice = true;
                            ViewBag.invalidWeight = false;
                            ViewBag.existProd = false;
                            return View(productVM2);
                        }
                        else
                        {
                            ViewBag.invalidPrice = false;
                        }
                        ViewBag.ShowMsg = false;
                        ViewBag.existProd = false;
                        _unitOfWork.Product.Add(productVM.Product);
                    }
                }
                else
                {
                    if (productVM.Product.Weight <= 0)
                    {
                        ViewBag.ShowMsg = true;
                        ViewBag.invalidWeight = true;
                        ViewBag.existProd = false;
                        return View(productVM2);
                    }
                    else
                    {
                        ViewBag.invalidWeight = false;
                        Product pr = _unitOfWork.Product.Get(productVM.Product.Id);
                        //if weight has changed
                        if(productVM.Product.Weight != pr.Weight)
                        {
                            if (productVM.Product.Weight < SD.weightFor1Rate && productVM.Product.Weight > 0)
                            {
                                productVM.Product.ShippingCharge = SD.priceFor1Rate;
                            }
                            else if (productVM.Product.Weight < SD.weightFor2Rate && productVM.Product.Weight >= SD.weightFor1Rate)
                            {
                                productVM.Product.ShippingCharge = SD.priceFor2Rate;
                            }
                            else if (productVM.Product.Weight < SD.weightForMaxRate && productVM.Product.Weight >= SD.weightFor2Rate)
                            {
                                productVM.Product.ShippingCharge = SD.priceForMaxRate;
                            }
                        }
                    }
                    if (productVM.Product.SellersCost <= 0)
                    {
                        ViewBag.ShowMsg = true;
                        ViewBag.invalidPrice = true;
                        ViewBag.invalidWeight = false;
                        ViewBag.existProd = false;
                        return View(productVM2);
                    }
                    else
                    {
                        ViewBag.invalidPrice = false;
                    }
                    //if status was rejected and there was a change on the list, put it back to pending.
                    if(productVM.Product.AdminApproval == SD.MerchProductStatusRejected)
                    {
                        productVM.Product.AdminApproval = SD.MerchProductStatusPending;
                    }
                    _unitOfWork.Product.update(productVM.Product);
                }
                ViewBag.ShowMsg = true;
                ViewBag.existProd = false;
                ViewBag.failed = false;
                _unitOfWork.Save();
                ViewBag.success = true;
                return View(productVM2);
            }
            else
            {
                ViewBag.failed = true;
            }
            ViewBag.ShowMsg = true;
            ViewBag.existProd = false;
            ViewBag.invalidWeight = false;
            ViewBag.invalidPrice = false;
            return View(productVM2);
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        [HttpPost]
        public JsonResult NetProfitCalc(double weight, double price)
        {
            double netProfit = 0.0;
            double addToMinimum = 0;
            List<string> strRes = new List<string>();
            if(weight <=0 || weight >= 1)
            {
                strRes.Add("Weight must be bigger then 0 and less then 1 pound");
                return Json(strRes);
            }
            else if (price <= 0)
            {
                strRes.Add("In order to calculate profit price cannot be empty");
                return Json(strRes);
            }
            else if (weight < SD.weightFor1Rate && weight > 0)
            {
                netProfit = price - (SD.FeesOfKTMerch * price) - (SD.priceFor1Rate+SD.addToKTMerchRate);
            }
            else if (weight < SD.weightFor2Rate && weight >= SD.weightFor1Rate)
            {
                netProfit = price - (SD.FeesOfKTMerch * price) - (SD.priceFor2Rate + SD.addToKTMerchRate);
            }
            else if (weight < SD.weightForMaxRate && weight >= SD.weightFor2Rate)
            {
                netProfit = price - (SD.FeesOfKTMerch * price) - (SD.priceForMaxRate + SD.addToKTMerchRate);
            }
            if ((price * SD.FeesOfKTMerch) < 0.8)
            {
                addToMinimum = 0.8 - (price * SD.FeesOfKTMerch);
                netProfit = netProfit - addToMinimum;
            }
            strRes.Add("Profit for each sold item is " + String.Format("{0:0.00}", netProfit) + "$. (10% Fee or 0.8$, highest between them + Shipping)");

            return Json(strRes);
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
