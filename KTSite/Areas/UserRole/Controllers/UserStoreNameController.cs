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
    public class UserStoreNameController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserStoreNameController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var userStoreNameList = _unitOfWork.UserStoreName.GetAll().Where(a=>a.UserNameId == returnUserNameId());
            ViewBag.existOrder = new Func<string, bool>(existOrder);
            return View(userStoreNameList);
        }
        public bool existOrder(string StoreId)
        {
            return _unitOfWork.Order.GetAll().Any(a => a.StoreNameId == Convert.ToInt32(StoreId));
        }
        public IActionResult AddStore()
        {

            ViewBag.UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.storeExist = true;
            ViewBag.ShowMsg = 0;
            return View();
        }
        public IActionResult UpdateStore(int Id)
        {
            ViewBag.UserNameId = returnUserNameId();
            UserStoreName userStoreName =
                  _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == Id).FirstOrDefault();

            ViewBag.ShowMsg = false;            
            return View(userStoreName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStore(UserStoreName userStoreName)
        {
            userStoreName.UserNameId =
            (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            bool storeExist = _unitOfWork.UserStoreName.GetAll().Where(q => q.UserNameId == userStoreName.UserNameId)
                   .Any(q => q.StoreName.Equals(userStoreName.StoreName, StringComparison.InvariantCultureIgnoreCase));
            userStoreName.UserName = User.Identity.Name;
            userStoreName.IsAdminStore = false;

            if (ModelState.IsValid)
            {
                if (!storeExist)
                {
                    _unitOfWork.UserStoreName.Add(userStoreName);

                    _unitOfWork.Save();
                }
                ViewBag.storeExist = storeExist;
                ViewBag.ShowMsg = 1;
                return View();


                //return RedirectToAction(nameof(Index));
            }
            return View(userStoreName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStore(UserStoreName userStoreName)
        {
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {   
                    ViewBag.ShowMsg = true;
                    _unitOfWork.UserStoreName.update(userStoreName);
                    _unitOfWork.Save();
                
            }
            return View(userStoreName);
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
