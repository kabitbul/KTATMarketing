using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
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
            string uNameId = returnUserNameId();
            ViewBag.getUserName =
                new Func<string, string>(getUserName);
            var payHistMerch = _unitOfWork.PaymentHistoryMerch.GetAll().Where(a=> a.UserNameId == uNameId).OrderByDescending(a => a.Id);
            return View(payHistMerch);
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        public string getUserName(string Id)
        {
            return _unitOfWork.ApplicationUser.Get(Id).Name;
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
