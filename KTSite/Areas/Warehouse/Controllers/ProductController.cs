using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Authorize(Roles = SD.Role_Warehouse)]
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
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ProductVM productVM = new ProductVM()
            {

                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
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
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (productVM.Product.Id != 0)
                {
                    if(productVM.Product.Weight <= 0)
                    {
                        ViewBag.InvalidWeight = 1;
                    }
                    else
                    {
                        Product pr = _unitOfWork.Product.Get(productVM.Product.Id);
                        //if weight has changed
                        if (productVM.Product.Weight != pr.Weight && productVM.Product.MerchType == SD.Role_KTMerch)
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
                        ViewBag.InvalidWeight = 0;
                        _unitOfWork.Product.update(productVM.Product);
                        _unitOfWork.Save();
                        ViewBag.success = true;
                    }
                }
                    
                //return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includePoperties:"Category").Where(a=> a.MerchType != SD.Role_ExMerch);
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
