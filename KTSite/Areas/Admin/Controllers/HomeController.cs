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

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            if(User.IsInRole(SD.Role_Admin))
            {
                IEnumerable<Notification> NotList = _unitOfWork.Notification.GetAll().Where(a => a.Visible).OrderByDescending(a => a.DateMsg);
                ViewBag.NotificationList = NotList;
                if (NotList != null && NotList.Count() > 0)
                {
                    ViewBag.NotificationEmpty = false;
                }
                else
                {
                    ViewBag.NotificationEmpty = true;

                }
                ViewBag.Name = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).
                    Select(q => q.Name).FirstOrDefault();
                ViewBag.OrdersFromChina =_unitOfWork.ChinaOrder.GetAll().
                    Where(a => a.DateOrdered.Date <= (DateTime.Now.AddDays(-45).Date)
                  && a.QuantityReceived == 0).Count();
                ViewBag.WarehouseBalanceLow = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).
                    Select(a => a.Balance).FirstOrDefault();
                ViewBag.CountPendingPayments = _unitOfWork.PaymentHistory.GetAll().
                    Where(a => a.Status == SD.PaymentStatusPending).Count();
                var product = _unitOfWork.Product.GetAll().Where(a => (a.InventoryCount + a.OnTheWayInventory) > 0);
                double totalInventoryValue = 0; 
                foreach(Product prod in product)
                {
                    if (!prod.OwnByWarehouse && prod.Id != 60 && prod.Id != 61)// Daniel Items (Dropshipping)
                    {
                        totalInventoryValue = totalInventoryValue + ((prod.InventoryCount + prod.OnTheWayInventory) * prod.Cost);
                    }
                }
                ViewBag.totalInventoryValue = totalInventoryValue;
                ViewBag.CountArrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll().
                    Where(a => !a.UpdatedByAdmin).Count();
                //stack chart user\admin
                List<DataPoint> dataPointsUser = new List<DataPoint>();
                List<DataPoint> dataPointsAdmin = new List<DataPoint>();
                getStackGraphData2(dataPointsUser, dataPointsAdmin);
                //getStackGraphData(true, dataPointsAdmin);
                ViewBag.DataPointsUser = JsonConvert.SerializeObject(dataPointsUser);
                ViewBag.DataPointsAdmin = JsonConvert.SerializeObject(dataPointsAdmin);
                return View();
            }
            else if (User.IsInRole(SD.Role_Users))
            {
                return Redirect("UserRole/Home");
            }
            else if (User.IsInRole(SD.Role_VAs))
            {
                return Redirect("VAs/Home");
            }
            else if (User.IsInRole(SD.Role_Warehouse))
            {
                return Redirect("Warehouse/Home");
            }
            //string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            //if (uNameId == null)
            // {
            return Redirect("Identity/Account/Login");
            // }

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
    }
}
