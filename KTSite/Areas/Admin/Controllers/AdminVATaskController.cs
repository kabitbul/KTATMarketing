using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminVATaskController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdminVATaskController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            ViewBag.getStore = new Func<int?, string>(getStore);
            ViewBag.getUserName = new Func<string, string>(getUserName);
            //get all tasks from VAs
            var adminVATask = _unitOfWork.adminVATask.GetAll().Join(_unitOfWork.ApplicationUser.GetAll().
                Where(a => a.Role == SD.Role_VAs),
                                             adminVATask => adminVATask.UserNameId,
                                             applicationUser => applicationUser.Id,
                                             (adminVATask, applicationUser) => new
                                             {
                                                 adminVATask
                                             }).Select(a => a.adminVATask);
            return View(adminVATask);
        }
        public IActionResult ShowAdminTask()
        {
            ViewBag.getStore = new Func<int?, string>(getStore);
            ViewBag.getUserName = new Func<string, string>(getUserName);
            //get all tasks from VAs
            var adminVATask = _unitOfWork.adminVATask.GetAll().Join(_unitOfWork.ApplicationUser.GetAll().
                Where(a => a.Role == SD.Role_Admin),
                                             adminVATask => adminVATask.UserNameId,
                                             applicationUser => applicationUser.Id,
                                             (adminVATask, applicationUser) => new
                                             {
                                                 adminVATask
                                             }).Select(a => a.adminVATask);
            return View(adminVATask);
        }
        public string getUserName(string userNameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == userNameId).Select(a => a.Name).FirstOrDefault();
        }
        public string getStore(int? storeId)
        {
            if (storeId == 0)
                return "All Stores";
            else
                return _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == storeId).Select(a => a.StoreName).FirstOrDefault();
        }
        public IActionResult AddTask(int? id)
        {

            ViewBag.UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).
                Select(q => q.Id)).FirstOrDefault();
            AdminVATaskVM adminVATaskVM = new AdminVATaskVM()
            {
                AdminVATask = new AdminVATask(),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(a=>a.IsAdminStore)
                     .Select(i => new SelectListItem
                {
                 Text = i.StoreName ,
                  Value = i.Id.ToString()
                })
            };
            if (id != null)
            {
                adminVATaskVM.AdminVATask = _unitOfWork.adminVATask.Get(id.GetValueOrDefault());
            }
            ViewBag.ShowMsg = false;
            ViewBag.success = false;
            return View(adminVATaskVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTask(AdminVATaskVM adminVATaskVM)
        {
            ViewBag.ShowMsg = true;
            if (adminVATaskVM.AllStores)
            {
                adminVATaskVM.AdminVATask.StoreId = 0;
            }
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).
                Select(q => q.Id)).FirstOrDefault();
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                if (adminVATaskVM.AdminVATask.Id == 0)
                {
                    _unitOfWork.adminVATask.Add(adminVATaskVM.AdminVATask);
                }
                else
                {
                    _unitOfWork.adminVATask.update(adminVATaskVM.AdminVATask);
                }
                    _unitOfWork.Save();
                ViewBag.success = true;
            }
            AdminVATaskVM adminVATaskVM1 = new AdminVATaskVM()
            {
                AdminVATask = new AdminVATask(),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore)
                    .Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
            };
            return View(adminVATaskVM1);
        }
        [HttpPost]
        public IActionResult ApproveTask(int[] Ids)
        {
            foreach(int Id in Ids)
            {
                AdminVATask adminVATask = _unitOfWork.adminVATask.GetAll().Where(a => a.Id == Id).FirstOrDefault();
                adminVATask.TaskCompleted = true;
                _unitOfWork.Save();
            }
            return View();
        }
        #region API CALLS
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.adminVATask.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.adminVATask.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
