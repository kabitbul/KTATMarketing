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

namespace KTSite.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Authorize(Roles = SD.Role_Warehouse)]
    public class ArrivingFromChinaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ArrivingFromChinaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var arrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll();
            ViewBag.getProductName =
               new Func<int, string>(getProductName);
            return View(arrivingFromChina);
        }
        public string getProductName(int ProductId)
        {
             return _unitOfWork.Product.GetAll().Where(a => a.Id == ProductId).Select(a => a.ProductName).FirstOrDefault();
        }
        public IActionResult AddArrivingFromChina()
        {
            ArrivingFromChinaVM arrivingFromChinaVM;
            arrivingFromChinaVM = new ArrivingFromChinaVM()
                {
                arrivingFromChina = new ArrivingFromChina(),
                    ProductList = _unitOfWork.Product.GetAll().OrderBy(a=>a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.ProductName,
                        Value = i.Id.ToString()
                    })
                };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            return View(arrivingFromChinaVM);
        }
        public IActionResult UpdateArrivingFromChina(long Id)
        {
            ArrivingFromChina arrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll().Where(a => a.Id == Id).FirstOrDefault();
            ArrivingFromChinaVM arrivingFromChinaVM = new ArrivingFromChinaVM()
                {
                   arrivingFromChina = arrivingFromChina,
                    ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).
                     Select(i => new SelectListItem
                     {
                         Text = i.ProductName,
                         Value = i.Id.ToString()
                     })
                };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            return View(arrivingFromChinaVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddArrivingFromChina(ArrivingFromChinaVM arrivingFromChinaVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            ArrivingFromChinaVM arrivingFromChinaVM2 = new ArrivingFromChinaVM()
            {
            arrivingFromChina = new ArrivingFromChina(),
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).
                Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                })
            };
            if (ModelState.IsValid)
            {

               _unitOfWork.ArrivingFromChina.Add(arrivingFromChinaVM.arrivingFromChina);
                    _unitOfWork.Save();
                ViewBag.success = true;
            }
            return View(arrivingFromChinaVM2);
            //return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateArrivingFromChina(ArrivingFromChinaVM arrivingFromChinaVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            ArrivingFromChinaVM arrivingFromChinaVM2 = new ArrivingFromChinaVM()
            {
                arrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll().Where(a => a.Id == arrivingFromChinaVM.arrivingFromChina.Id).FirstOrDefault(),
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).
                    Select(i => new SelectListItem
                    {
                        Text = i.ProductName,
                        Value = i.Id.ToString()
                    })
            };
            if (ModelState.IsValid)
            {
                    _unitOfWork.ArrivingFromChina.update(arrivingFromChinaVM.arrivingFromChina);
                    _unitOfWork.Save();
                ViewBag.success = true;
            }
            return View(arrivingFromChinaVM2);
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
