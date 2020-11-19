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
    public class UserGuidelineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserGuidelineController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<UserGuideline> userGuidelines = _unitOfWork.UserGuideline.GetAll();
            return View(userGuidelines);
        }
        public IActionResult UpsertUserGuideline(long? id)
        {
            ViewBag.ShowMsg = 0;
            UserGuideline userGuideline = new UserGuideline();
            if(id == null)//create
            {
                return View(userGuideline);
            }
            userGuideline = _unitOfWork.UserGuideline.Get(id.GetValueOrDefault());
            if (userGuideline == null)
            {
                return NotFound();
            }
            return View(userGuideline);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertUserGuideline(UserGuideline userGuideline)
        {
            if (ModelState.IsValid)
            {
                if (userGuideline.Id == 0)
                {
                  _unitOfWork.UserGuideline.Add(userGuideline);
                }
                else
                {
                    _unitOfWork.UserGuideline.update(userGuideline);
                }
                ViewBag.ShowMsg = 1;
                _unitOfWork.Save();
                return View(userGuideline);
            }
            ViewBag.ShowMsg = 1;
            return View(userGuideline);
        }
        #region API CALLS
        [HttpDelete]
        public IActionResult Delete(long id)
        {
            var objFromDb = _unitOfWork.UserGuideline.GetAll().Where(a => a.Id == id).FirstOrDefault();
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.UserGuideline.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
