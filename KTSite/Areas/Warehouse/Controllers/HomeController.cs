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

namespace KTSite.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
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
            ViewBag.Name = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
            ViewBag.Balance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).Select(a=>a.Balance).FirstOrDefault();
            string WarehouseUserId = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).
                Select(a => a.UserNameId).FirstOrDefault();
            var paymentHistory = _unitOfWork.PaymentHistory.GetAll().Where(a => a.UserNameId == WarehouseUserId).OrderByDescending(a => a.Id).FirstOrDefault();
            if (paymentHistory != null)
            {
                ViewBag.PayDate = paymentHistory.PayDate;
                ViewBag.Amount = paymentHistory.Amount;
            }
            else
            {
                ViewBag.PayDate = null;
                ViewBag.Amount = 0;
            }
            ViewBag.ExistProgress = _unitOfWork.Order.GetAll().Any(a => a.OrderStatus == SD.OrderStatusInProgress);
            int WaitingForProcess = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusAccepted).Count();
            ViewBag.WaitingForProcess = WaitingForProcess;
            int WaitingForReturnLabel = _unitOfWork.ReturnLabel.GetAll().Where(a => a.FileURL == null).Count();
            int missingWeightCount = _unitOfWork.Product.GetAll().Where(a => a.Weight == 0 && a.InventoryCount > 0).Count();
            ViewBag.missingWeightCount = missingWeightCount;
            ViewBag.WaitingForReturnLabel = WaitingForReturnLabel;
            DateTime iterateDate = DateTime.Now.AddDays(-30);
                List<DataPoint> dataPoints = new List<DataPoint>();
                var result = _unitOfWork.Order.GetAll().Where(a=>a.OrderStatus != SD.OrderStatusCancelled).GroupBy(a => a.UsDate)
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


		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
