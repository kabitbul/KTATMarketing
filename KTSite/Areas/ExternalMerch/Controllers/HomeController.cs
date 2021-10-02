using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KTSite.Models;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Utility;
using Newtonsoft.Json;

namespace KTSite.Areas.ExternalMerch.Controllers
{
    [Area("ExternalMerch")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string userNameId = returnUserNameId();
            PaymentBalanceMerch pb = _unitOfWork.PaymentBalanceMerch.GetAll().Where(a => a.UserNameId == userNameId).FirstOrDefault();
            if (pb == null)
            {
                PaymentBalanceMerch paymentBalance = new PaymentBalanceMerch();
                paymentBalance.UserNameId = userNameId;
                paymentBalance.Balance = 0.0;
                paymentBalance.MerchType = SD.Role_ExMerch;

                _unitOfWork.PaymentBalanceMerch.Add(paymentBalance);
                _unitOfWork.Save();
            }
            var appUser = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.Name = appUser.Name;
            int WaitingForProcess = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusAccepted && a.MerchId == returnUserNameId()).Count();
            ViewBag.WaitingForProcess = WaitingForProcess;
            int WaitingForReturnLabel = _unitOfWork.ReturnLabel.GetAll().Where(a => a.FileURL == null && a.MerchId == returnUserNameId()).Count();
            ViewBag.WaitingForReturnLabel = WaitingForReturnLabel;
            ViewBag.NumOfComplaints = _unitOfWork.Complaints.GetAll().Where(a => a.MerchId == returnUserNameId() && !a.Solved).Count();
            var product = _unitOfWork.Product.GetAll().Where(a => (a.InventoryCount ) > 0 && a.AdminApproval == SD.MerchProductStatusApproved &&  a.MerchId == appUser.Id);
                double totalCountInventory = 0; 
                foreach(Product prod in product)
                {
                   // if (!prod.OwnByWarehouse && prod.Id != 60 && prod.Id != 61)// Daniel Items (Dropshipping)
                   // {
                    totalCountInventory = totalCountInventory + (prod.InventoryCount + prod.OnTheWayInventory) ;
                   // }
                }
                ViewBag.totalCountInventory = totalCountInventory;
            //ViewBag.CountArrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll().
            //    Where(a => !a.UpdatedByAdmin && a.Quantity != 0).Count();
            //Graph Data
            DateTime iterateDate = DateTime.Now.AddMonths(-1);
            List<DataPoint> dataPoints = new List<DataPoint>();
            var result = _unitOfWork.Order.GetAll().Where(a => a.MerchId == userNameId && a.OrderStatus != SD.OrderStatusCancelled)
                .GroupBy(a => a.UsDate)
                   .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).ToList();
            while (iterateDate <= DateTime.Now)
            {
                if (result.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                {
                    dataPoints.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                          result.Find(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
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
        public bool returnIsRefunded(long OrderId)
        {
            return _unitOfWork.Refund.GetAll().Any(a => a.OrderId == OrderId);
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
        public void getStackGraphData2(List<DataPoint> listUser, List<DataPoint> listAdmin)
        {
            DateTime iterateDate = DateTime.Now.AddMonths(-1);

                var result = _unitOfWork.Order.GetAll().Where(a=>a.OrderStatus != SD.OrderStatusCancelled && 
                                                              a.UsDate >= DateTime.Now.AddMonths(-1)).
                                                              GroupBy(a => new { a.UsDate, a.IsAdmin })
                          .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).OrderBy(a => a.date.UsDate).ToList();
                while (iterateDate <= DateTime.Now)
                {
                    if (result.Exists(x => x.date.UsDate.ToString("dd/MM") == iterateDate.ToString("dd/MM") && x.date.IsAdmin) )
                    {
                    listAdmin.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                              result.Find(x => x.date.UsDate.ToString("dd/MM") == iterateDate.ToString("dd/MM") && x.date.IsAdmin).total));
                    }
                    else
                    {
                    listAdmin.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),0));
                    }
                    if (result.Exists(x => x.date.UsDate.ToString("dd/MM") == iterateDate.ToString("dd/MM") && !x.date.IsAdmin))
                {
                    listUser.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                              result.Find(x => x.date.UsDate.ToString("dd/MM") == iterateDate.ToString("dd/MM") && !x.date.IsAdmin).total));
                }
                else
                {
                    listUser.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                }
                iterateDate = iterateDate.AddDays(1);
                }
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
    }
}
