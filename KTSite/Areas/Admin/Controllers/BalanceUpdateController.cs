using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Collections.Generic;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BalanceUpdateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BalanceUpdateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<BalanceUpdatesList> balanceUpdateList = 
                  _unitOfWork.balanceUpdate.GetAllBalanceUpdates();
            return View(balanceUpdateList);
        }
        public IActionResult AddBalanceUpdate()
        {
            ViewBag.success = true;
            ViewBag.ShowMsg = false;
            
            BalanceUpdateVM balanceUpdateVM = new BalanceUpdateVM()
            {
                balanceUpdate = new BalanceUpdatesList(),
                BalanceUpdateActionList = SD.AddOrDeduct,
                UsersList = _unitOfWork.balanceUpdate.GetUsersList().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.UserNameId
                })
            };
            
            ViewBag.sysDate = DateTime.Now;
            ViewBag.failed = false;
            return View(balanceUpdateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBalanceUpdate(BalanceUpdateVM balanceUpdateVM)
        {
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
              BalanceUpdateVM balanceUpdateVM1 = new BalanceUpdateVM()
                {
                    balanceUpdate = new BalanceUpdatesList(),
                   BalanceUpdateActionList = SD.AddOrDeduct,
                UsersList = _unitOfWork.balanceUpdate.GetUsersList().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.UserNameId
                })
                };
            if (ModelState.IsValid)
            {
            if(balanceUpdateVM.balanceUpdate.Action == "Add")
              {
                _unitOfWork.balanceUpdate.UpdateUserBalance(balanceUpdateVM.balanceUpdate.Amount,
                                                            balanceUpdateVM.balanceUpdate.UserNameId);
              }
            else if(balanceUpdateVM.balanceUpdate.Action == "Deduct")
             {   _unitOfWork.balanceUpdate.UpdateUserBalance((balanceUpdateVM.balanceUpdate.Amount*-1),
                                                            balanceUpdateVM.balanceUpdate.UserNameId);
             }
             else
             {
               return View(balanceUpdateVM1);
              }
               _unitOfWork.balanceUpdate.InsertBalanceUpdate(balanceUpdateVM.balanceUpdate);
            
                _unitOfWork.Save();
                ViewBag.success = true;
            }
                
            return View(balanceUpdateVM1);
        }   
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll();
            return Json(new { data = allObj });
        }

        #endregion
    }
}
