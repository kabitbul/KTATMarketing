using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using KTSite.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using KTSite.DataAccess.Migrations;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class PaymentBalanceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentBalanceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           var userNameIdList = _unitOfWork.PaymentBalance.GetAll();
            ViewBag.getUserName =
              new Func<string, string>(getUserName);
            ViewBag.getName =
            new Func<string, string>(getName);
            ViewBag.IsUserRole =  new Func<int, bool>(IsUserRole); 
            return View(userNameIdList);

        }
         public IActionResult Insert()
         {
            ViewBag.showMsg = false;
            ViewBag.success = true;

            PaymentBalanceVM paymentBalanceVM = new PaymentBalanceVM()
            {
            paymentBalances = new PaymentBalance(),
                UsersList = _unitOfWork.ApplicationUser.GetAll().
                Where(a => !_unitOfWork.PaymentBalance.GetAll().Any(p => p.UserNameId == a.Id)).Select(i => new SelectListItem
                //_unitOfWork.ApplicationUser.GeAllUsersWithoutrecInPayBalance().Select(i => new SelectListItem
                {
                    Text = i.Name +"-"+i.UserName,
                    Value = i.Id.ToString()
                })
            };
                   return View(paymentBalanceVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Insert(PaymentBalanceVM paymentBalanceVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            var appUser = _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == paymentBalanceVM.paymentBalances.UserNameId).FirstOrDefault();
            if(appUser.Role == SD.Role_Warehouse)
            {
                paymentBalanceVM.paymentBalances.IsWarehouseBalance = true;
            }
            ViewBag.showMsg = true;
            paymentBalanceVM.UsersList = _unitOfWork.ApplicationUser.GetAll().
                Where(a => !_unitOfWork.PaymentBalance.GetAll().Any(p => p.UserNameId == a.Id)).Select(i => new SelectListItem
                {
                Text = i.UserName,
                Value = i.Id.ToString()
            });
            if (ModelState.IsValid)
            {
                _unitOfWork.PaymentBalance.Add(paymentBalanceVM.paymentBalances);
                _unitOfWork.Save();
                ViewBag.success = true;
                return View(paymentBalanceVM);
            }
            return View(paymentBalanceVM);
        }
        public string getUserName(string userNameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == userNameId).Select(a => a.UserName).FirstOrDefault();
        }
        public string getName(string userNameId)
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.Id == userNameId).Select(q => q.Name)).FirstOrDefault();
        }
        public bool IsUserRole(int Id)
        {
            string userName = (_unitOfWork.PaymentBalance.GetAll().Where(q => q.Id == Id).Select(q => q.UserNameId)).FirstOrDefault();
            return
                _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == userName).Any(a => a.Role == SD.Role_Users);
        }
        [HttpPost]
        public IActionResult EditAllow(int[] Ids)
        {
            foreach (int Id in Ids)
            {
                PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.Id == Id).FirstOrDefault();
                if (paymentBalance.AllowNegativeBalance)
                {
                    paymentBalance.AllowNegativeBalance = false;
                }
                else
                {
                    paymentBalance.AllowNegativeBalance = true;
                }
                _unitOfWork.Save();
            }
            return View();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.PaymentBalance.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.PaymentBalance.GetAll().Where(a => a.Id == id).FirstOrDefault() ;
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.PaymentBalance.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
