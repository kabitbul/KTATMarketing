using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using Microsoft.IdentityModel.Protocols;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UsersForAPIController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public UsersForAPIController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            //get all tasks from VAs
            var usersForAPI = _unitOfWork.UsersForAPI.GetAll();
            return View(usersForAPI);
        }
        public IActionResult Upsert(int? id)
        {
            ViewBag.existProd = false;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            UsersForAPIVM usersForAPIVM = new UsersForAPIVM()
            {
                usersForAPI = new UsersForAPI(),
                AppUsersList = _unitOfWork.ApplicationUser.GetAll().Where(a=> a.Role == SD.Role_Users && (a.LockoutEnd == null || a.LockoutEnd < DateTime.Now)
                               ).Where(a => !_unitOfWork.UsersForAPI.GetAll().Any(p => p.UserId == a.Id))
                     .Select(i => new SelectListItem
                     {
                         Text = i.Name + " " + i.UserName,
                         Value = i.Id.ToString()
                     })
            };
            if (id == null)//create
            {
                usersForAPIVM.usersForAPI.Active = true;
                return View(usersForAPIVM);
            }
            UsersForAPI uAPI = _unitOfWork.UsersForAPI.Get(id.GetValueOrDefault());

            UsersForAPIVM usersForAPIVM2 = new UsersForAPIVM()
            {
                usersForAPI = _unitOfWork.UsersForAPI.Get(id.GetValueOrDefault()),// new UsersForAPI(),
                AppUsersList = _unitOfWork.ApplicationUser.GetAll().Where(a => a.Role == SD.Role_Users && (a.LockoutEnd == null || a.LockoutEnd < DateTime.Now)
                               ).Where(a => !_unitOfWork.UsersForAPI.GetAll().Any(p => p.UserId == a.Id)|| a.Id == uAPI.UserId)
                     .Select(i => new SelectListItem
                     {
                         Text = i.Name + " " + i.UserName,
                         Value = i.Id.ToString()
                     })
            };
            
            
            if (usersForAPIVM2.usersForAPI == null)
            {
                return NotFound();
            }
            return View(usersForAPIVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(UsersForAPIVM usersForAPIVM)
        {
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
            UsersForAPIVM usersForAPIVM2 = new UsersForAPIVM()
            {
                usersForAPI = new UsersForAPI(),
                AppUsersList = _unitOfWork.ApplicationUser.GetAll().Where(a => a.Role == SD.Role_Users && (a.LockoutEnd == null || a.LockoutEnd < DateTime.Now)
                                ).Where(a => !_unitOfWork.UsersForAPI.GetAll().Any(p => p.UserId == a.Id))
                      .Select(i => new SelectListItem
                      {
                          Text = i.Name + " " + i.UserName,
                          Value = i.Id.ToString()
                      })
            };
            if (ModelState.IsValid)
            {
                if (usersForAPIVM.usersForAPI.Id == 0)
                {
                    bool existUser = _unitOfWork.UsersForAPI.GetAll().Any(a => a.Name.ToLower() == usersForAPIVM.usersForAPI.Name.ToLower());
                    if (existUser)
                    {
                        ViewBag.existUser = true;
                        return View(usersForAPIVM2);
                    }
                    else
                    {
                        ViewBag.existUser = false;
                        usersForAPIVM.usersForAPI.Name = _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == usersForAPIVM.usersForAPI.UserId).Select(a => a.Name).FirstOrDefault();
                        _unitOfWork.UsersForAPI.Add(usersForAPIVM.usersForAPI);
                    }
                }
                else
                {
                    _unitOfWork.UsersForAPI.update(usersForAPIVM.usersForAPI);
                }
                ViewBag.ShowMsg = true;
                ViewBag.existUser = false;
                ViewBag.failed = false;
                _unitOfWork.Save();
                ViewBag.success = true;
                return View(usersForAPIVM2);
            }
            else
            {
                ViewBag.failed = true;
            }
            ViewBag.ShowMsg = true;
            ViewBag.existUser = false;
            return View(usersForAPIVM2);
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _unitOfWork.ApplicationUser.GetAll();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach(var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var appUser = _unitOfWork.ApplicationUser.Get(id);
            if(appUser == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (appUser.LockoutEnd != null && appUser.LockoutEnd > DateTime.Now)
            {
                appUser.LockoutEnd = DateTime.Now;
            }
            else
            {
                appUser.LockoutEnd = DateTime.Now.AddYears(100);
            }
                _unitOfWork.ApplicationUser.update(appUser);
                    _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful." });
        }
        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var objFromDb = _unitOfWork.ApplicationUser.Get(id);
        //    if(objFromDb == null)
        //    {
        //        return Json(new { success = false, message = "Error While Deleting" });
        //    }
        //    _unitOfWork.ApplicationUser.Remove(objFromDb);
        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "Delete Successfull" });
        //}

        #endregion
    }
}
