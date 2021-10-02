using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class PaymentHistoryMerchController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentHistoryMerchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult MerchList()
        {
           
            var merchUsersList = _unitOfWork.ApplicationUser.GetAll().Where(a => a.Role == SD.Role_ExMerch || a.Role == SD.Role_KTMerch);
            ViewBag.getPaymentDate =
                  new Func<string, DateTime>(getPaymentDate);
            ViewBag.getAmount =
                  new Func<string, String>(getAmount);
            ViewBag.getPaymentMethod =
                  new Func<string, string>(getPaymentMethod);
            ViewBag.getSentTo =
              new Func<string, string>(getSentTo);
            ViewBag.getSentFrom =
              new Func<string, string>(getSentFrom);
            return View(merchUsersList);
        }
        public string getSentFrom(string Id)
        {
            PaymentHistoryMerch pm = _unitOfWork.PaymentHistoryMerch.GetAll().Where(a => a.UserNameId == Id).OrderByDescending(a => a.Id).FirstOrDefault();
            if (pm == null)
                return null;
            else
                return pm.SentFromAddress;
        }
        public string getSentTo(string Id)
        {
            PaymentHistoryMerch pm = _unitOfWork.PaymentHistoryMerch.GetAll().Where(a => a.UserNameId == Id).OrderByDescending(a => a.Id).FirstOrDefault();
            if (pm == null)
                return null;
            else
                return pm.SentToAddress;
        }
        public string getPaymentMethod(string Id)
        {
            PaymentHistoryMerch pm = _unitOfWork.PaymentHistoryMerch.GetAll().Where(a => a.UserNameId == Id).OrderByDescending(a => a.Id).FirstOrDefault();
            if (pm == null)
                return null;
            else
                return pm.PaymentMethod;
        }
        public DateTime getPaymentDate(string Id)
        {
            PaymentHistoryMerch pm = _unitOfWork.PaymentHistoryMerch.GetAll().Where(a => a.UserNameId == Id).OrderByDescending(a=> a.Id).FirstOrDefault();
            if (pm == null)
                return DateTime.MinValue;
            else
                return pm.PayDate;
        }
        public string getAmount(string Id)
        {
            PaymentHistoryMerch pm = _unitOfWork.PaymentHistoryMerch.GetAll().Where(a => a.UserNameId == Id).OrderByDescending(a => a.Id).FirstOrDefault();
            if (pm == null)
                return null;
            else
                return pm.Amount.ToString();
        }
        public IActionResult Index()
        {
            ViewBag.getUserName =
                new Func<string, string>(getUserName);
            var payHistMerch = _unitOfWork.PaymentHistoryMerch.GetAll().OrderByDescending(a => a.Id);
            return View(payHistMerch);
        }
        public IActionResult AddPaymentMerch(string? Id)
        {

            PaymentHistoryMerchVM paymentHistoryMerchVM;
            string uNameId = "";
            string uName = "";

            var appUser = _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == Id).FirstOrDefault();
            uName = appUser.Name;
            if (Id != null)
            {
                paymentHistoryMerchVM = new PaymentHistoryMerchVM()
                {
                    PaymentHistoryMerch = new PaymentHistoryMerch(),
                    merchName = uName,
                    PaymentAddress = _unitOfWork.paymentMethodMerch.GetAll().Where(a => a.UserNameId == Id).
                  Select(i => new SelectListItem
                  {
                      Text = i.PaymentTypeAddress + "- " + i.PaymentType + "-preferred: " + i.PrefferdMethod,
                      Value = i.PaymentTypeAddress
                  }),
                    PaymentMethod = SD.paymentType,
                    userNameId = Id,
                    merchType = appUser.Role
                 
                };
                ViewBag.ShowMsg = false;
                ViewBag.success = true;
                return View(paymentHistoryMerchVM);
            }

            ViewBag.UNameId = uNameId;
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = false;
            ViewBag.failed = false;
            ViewBag.success = true;
            return View();
        }
        public IActionResult ShowMerchPayment()
        {
            ViewBag.getUserName =
                new Func<string, string>(getUserName);
            var PaymentHistoryMerch = _unitOfWork.PaymentHistoryMerch.GetAll();
            
            return View(PaymentHistoryMerch);
        }
        public string getUserName(string Id)
        {
            return _unitOfWork.ApplicationUser.Get(Id).Name;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPaymentMerch(PaymentHistoryMerchVM paymentHistoryMerchVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                //add to history
                _unitOfWork.PaymentHistoryMerch.Add(paymentHistoryMerchVM.PaymentHistoryMerch);
                _unitOfWork.Save();
                //subtract balance from merch balance
                var merchBalance = _unitOfWork.PaymentBalanceMerch.GetAll().Where(a => a.UserNameId == paymentHistoryMerchVM.PaymentHistoryMerch.UserNameId).FirstOrDefault();
                merchBalance.Balance = merchBalance.Balance - paymentHistoryMerchVM.PaymentHistoryMerch.Amount;
                _unitOfWork.Save();
                
                ViewBag.success = true;


                //return RedirectToAction(nameof(Index));
            }
            var appUser = _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == paymentHistoryMerchVM.PaymentHistoryMerch.UserNameId).FirstOrDefault();
            PaymentHistoryMerchVM paymentHistoryMerchVM2 = new PaymentHistoryMerchVM()
            {
                PaymentHistoryMerch = new PaymentHistoryMerch(),
                merchName = appUser.Name,
                PaymentAddress = _unitOfWork.paymentMethodMerch.GetAll().Where(a => a.UserNameId == appUser.Id).
                  Select(i => new SelectListItem
                  {
                      Text = i.PaymentTypeAddress + "- " + i.PaymentType + "-preferred: " + i.PrefferdMethod,
                      Value = i.Id.ToString()
                  }),
                PaymentMethod = SD.paymentType,
                userNameId = appUser.Id,
                merchType = appUser.Role

            };
            return View(paymentHistoryMerchVM2);
        }
        #region API CALLS
        
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.PaymentHistory.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.PaymentHistory.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
