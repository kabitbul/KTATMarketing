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
            IEnumerable<Product> product = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName);
            ViewBag.getCategoryName =
              new Func<int, string>(getCategoryName);
            ViewBag.Profit =
              new Func<int, string>(getProfit);
            return View(product);
        }
        public string getProfit(int productId)
        {
            double precent;
            double dollar;
            Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            precent = ((product.SellersCost - product.Cost - SD.shipping_cost) / product.Cost) * 100;
            dollar = product.SellersCost - product.Cost - SD.shipping_cost;
            return precent.ToString("0.00") + "%(" + dollar.ToString("0.00") + "$)";

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
                        ViewBag.existProd = true;
                    }
                    else
                    {
                        ViewBag.existProd = false;
                        _unitOfWork.Product.Add(productVM.Product);
                    }
                }
                else
                {
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
            return View(productVM2);
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
