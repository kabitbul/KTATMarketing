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
    public class AdminStoreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AdminStoreController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var userStoreNameList = _unitOfWork.UserStoreName.GetAll();
            return View(userStoreNameList);
        }
        public IActionResult AddAdminStore()
        {

            ViewBag.UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.storeExist = true;
            ViewBag.ShowMsg = false;
            return View();
        }
        public IActionResult AllUsersStores()
        {
            var userStoreNameList = _unitOfWork.UserStoreName.GetAll();
            return View(userStoreNameList);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAdminStore(UserStoreName userStoreName)
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            userStoreName.UserNameId = uNameId;

            ViewBag.UNameId = uNameId;
            bool storeExist = _unitOfWork.UserStoreName.GetAll().Where(q => q.IsAdminStore)
                   .Any(q => q.StoreName.Equals(userStoreName.StoreName, StringComparison.InvariantCultureIgnoreCase));
            userStoreName.UserName = User.Identity.Name;
            userStoreName.IsAdminStore = true;
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {
                if (!storeExist)
                {
                    _unitOfWork.UserStoreName.Add(userStoreName);

                    _unitOfWork.Save();
                }
                ViewBag.storeExist = storeExist;
                ViewBag.failed = false;
                return View();
            }
            ViewBag.storeExist = false;
            ViewBag.failed = true;
            return View();
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAllAdmin()
        {
            var allObj = _unitOfWork.UserStoreName.GetAll().Where(a=> a.IsAdminStore);
            return Json(new { data = allObj });
        }
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var allObj = _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore == false);
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.UserStoreName.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.UserStoreName.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
