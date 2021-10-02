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

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
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
            //IEnumerable<Notification> NotList = _unitOfWork.Notification.GetAll().Where(a => a.Visible).OrderByDescending(a => a.DateMsg);
            //ViewBag.NotificationList = NotList;
            //if (NotList != null && NotList.Count() > 0)
            //{
            //    ViewBag.NotificationEmpty = false;
            //}
            //else
            //{
            //    ViewBag.NotificationEmpty = true;

            //}
            //if a new user, create payment balance 
            PaymentBalanceMerch pb = _unitOfWork.PaymentBalanceMerch.GetAll().Where(a => a.UserNameId == userNameId).FirstOrDefault();
            if (pb == null)
            {
                PaymentBalanceMerch paymentBalance = new PaymentBalanceMerch();
                paymentBalance.UserNameId = userNameId;
                paymentBalance.Balance = 0;
                paymentBalance.MerchType = SD.Role_KTMerch;

                _unitOfWork.PaymentBalanceMerch.Add(paymentBalance);
                _unitOfWork.Save();
            }
            var appUser = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.Name = appUser.Name;
                //ViewBag.OrdersFromChina =_unitOfWork.ChinaOrder.GetAll().
                //    Where(a => a.DateOrdered.Date <= (DateTime.Now.AddDays(-45).Date)
                //  && a.QuantityReceived == 0).Count();
                //ViewBag.WarehouseBalanceLow = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).
                //    Select(a => a.Balance).FirstOrDefault();
                //ViewBag.CountPendingPayments = _unitOfWork.PaymentHistory.GetAll().
                //    Where(a => a.Status == SD.PaymentStatusPending).Count();
                //ViewBag.TaskFromVA = _unitOfWork.adminVATask.GetAll().Where(q=>!q.TaskCompleted).
                //    Join(_unitOfWork.ApplicationUser.GetAll().
                //Where(a => a.Role == SD.Role_VAs),
                //                             adminVATask => adminVATask.UserNameId,
                //                             applicationUser => applicationUser.Id,
                //                             (adminVATask, applicationUser) => new
                //                             {
                //                                 adminVATask
                //                             }).Select(a => a.adminVATask).Count();
                //ViewBag.ComplaintsPending = _unitOfWork.Complaints.GetAll().Where(a => !a.WarehouseResponsibility && !a.Solved).Count();
                //ViewBag.ReturnLabelRefund = _unitOfWork.ReturnLabel.GetAll().Where(a => a.ReturnDelivered && !returnIsRefunded(a.OrderId)).Count();
                var product = _unitOfWork.Product.GetAll().Where(a => (a.InventoryCount + a.OnTheWayInventory) > 0 && a.AdminApproval == SD.MerchProductStatusApproved && a.MerchId == appUser.Id);
                double totalCountInventory = 0; 
                foreach(Product prod in product)
                {
                    if (!prod.OwnByWarehouse && prod.Id != 60 && prod.Id != 61)// Daniel Items (Dropshipping)
                    {
                    totalCountInventory = totalCountInventory + (prod.InventoryCount + prod.OnTheWayInventory) ;
                    }
                }
                ViewBag.totalCountInventory = totalCountInventory;
            var pm = _unitOfWork.PaymentHistoryMerch.GetAll().Where(a => a.UserNameId == userNameId);
            if (pm.Count() > 0)
            {
                long pmId = pm.Max(a => a.Id);
                PaymentHistoryMerch pm2 =  _unitOfWork.PaymentHistoryMerch.Get(pmId);
                ViewBag.LastPaid = pm2.PayDate;
                ViewBag.AmountPaid = pm2.Amount;
            }
            
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
