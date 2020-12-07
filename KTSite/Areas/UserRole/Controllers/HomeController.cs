using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KTSite.Models;
using Newtonsoft.Json;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Utility;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            int PendingCount = 0;
            double PendingAmount = 0;
            ViewBag.NotificationList = _unitOfWork.Notification.GetAll().Where(a => a.Visible);
            ViewBag.Name = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
            ViewBag.Balance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == returnUserNameId()).Select(a=>a.Balance).FirstOrDefault();
            var paymentHistory = _unitOfWork.PaymentHistory.GetAll().Where(a => a.UserNameId == returnUserNameId() && a.Status == SD.PaymentStatusPending);
            foreach (PaymentHistory paymentHist in paymentHistory)
            {
                PendingCount++;
                PendingAmount = PendingAmount + paymentHist.Amount;
            }
            ViewBag.PendingCount = PendingCount;
            ViewBag.PendingAmount = PendingAmount;
            //Graph Data
            DateTime iterateDate = DateTime.Now.AddMonths(-1);
                List<DataPoint> dataPoints = new List<DataPoint>();
            string userNameId =returnUserNameId();
                var result = _unitOfWork.Order.GetAll().Where(a=>a.UserNameId == userNameId).GroupBy(a => a.UsDate)
                       .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).ToList();
                while (iterateDate <= DateTime.Now)
                {
                  if (result.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                  {
                    dataPoints.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                          result.Find(x=> x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
                  }
                  else
                {
                    dataPoints.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                }
                    iterateDate = iterateDate.AddDays(1);
                }

                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
                return View();
            }
            public string returnUserNameId()
            {
                return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            }


		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult MyBalance()
        {
            PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().FirstOrDefault();
        return PartialView("_TotalBalance", paymentBalance);
        }
    }
}
