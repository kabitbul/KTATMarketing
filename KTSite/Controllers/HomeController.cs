using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
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
            string UNameId = (_unitOfWork.ApplicationUser.GetAll().
                Where(q => q.UserName == User.Identity.Name).
                Select(q => q.Id)).FirstOrDefault();
            double Balance =
            _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == UNameId).
            Select(a => a.Balance).FirstOrDefault();
            return Content(Balance.ToString("0.00")+"$");
        }
        public IActionResult GetBalanceWarehouse()
        {
            double Balance =
                       _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).
                       Select(a => a.Balance).FirstOrDefault();
            return Content(Balance.ToString("0.00") + "$");
        }
        //public JsonResult GetUserDetails()
        //{
        //    string uName =
        //        (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Email).FirstOrDefault());
        //    return Content(uName);
        //}
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