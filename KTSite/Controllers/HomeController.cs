using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Mvc;

namespace KTSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult GetBalanceUser()
        {
            var appUser = (_unitOfWork.ApplicationUser.GetAll().
                Where(q => q.UserName == User.Identity.Name).FirstOrDefault());
            string UNameId = appUser.Id;
            if(appUser.Role == SD.Role_Users)
            {
                double Balance =
            _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == UNameId).
            Select(a => a.Balance).FirstOrDefault();
                return Content(Balance.ToString("0.00") + "$");
            }
            else//merch
            {
                double Balance =
            _unitOfWork.PaymentBalanceMerch.GetAll().Where(a => a.UserNameId == UNameId).
            Select(a => a.Balance).FirstOrDefault();
                return Content(Balance.ToString("0.00") + "$");
            }
            
        }
        public IActionResult GetBalanceWarehouse()
        {
            double Balance =
                       _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).
                       Select(a => a.Balance).FirstOrDefault();
            return Content(Balance.ToString("0.00") + "$");
        }
        public IActionResult GetHelloUser()
        {
            string nameU =
                       _unitOfWork.ApplicationUser.GetAll().
                       Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
            return Content("Hello " + nameU +"!"); ;
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        public IActionResult GetUserPendingPayment()
        {
            string userNameId = returnUserNameId();
            int PendingCount = 0;
            double PendingAmount = 0;
            var paymentHistory = _unitOfWork.PaymentHistory.GetAll().Where(a => a.UserNameId == userNameId && a.Status == SD.PaymentStatusPending);
            foreach (PaymentHistory paymentHist in paymentHistory)
            {
                PendingCount++;
                PendingAmount = PendingAmount + paymentHist.Amount;
            }
            if(PendingCount > 0)
            {
                return Content("You Have " + PendingAmount.ToString("0.00") + "$ waiting for Admin Approval"); 
            }
            else
            {
                return Content("No Pending Payments");
            }
        }
        public IActionResult GetAdminNotHeader()
        {
            IEnumerable<Notification> NotList = _unitOfWork.Notification.GetAll().Where(a => a.Visible).OrderByDescending(a => a.DateMsg);
            if (NotList != null && NotList.Count() > 0)
            {
                return Content("Important Admin Notifications:");
            }
            else
            {
                return Content("");
            }
        }
        public JsonResult GetAdminNotifications()
        {
            List<string> strRes = new List<string>();
            IEnumerable<Notification> NotList = _unitOfWork.Notification.GetAll().Where(a => a.Visible).OrderByDescending(a => a.DateMsg);
            foreach (Notification notif in NotList)
            {
                if (notif.DateMsg.Date >= DateTime.Now.Date.AddDays(-1))
                {
                    strRes.Add("1"+notif.Message);
                }
                else
                {
                    strRes.Add("0" + notif.Message);
                }
            }
                return Json(strRes);
        }
        public JsonResult GetUserDetails(DateTime fromDate, DateTime toDate)
        {
            List<string> strRes = new List<string>();
            var appUser = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).FirstOrDefault();
            strRes.Add(appUser.Name);
            strRes.Add(appUser.Email);
            DateTime currDate = DateTime.Now;
            strRes.Add(((DateTimeOffset)currDate).ToUnixTimeSeconds().ToString());
            return Json(strRes);
        }
    }
}