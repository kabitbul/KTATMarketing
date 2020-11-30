using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
                getStackGraphData(false, dataPointsUser);
                getStackGraphData(true, dataPointsAdmin);
                ViewBag.DataPointsUser = JsonConvert.SerializeObject(dataPointsUser);
                ViewBag.DataPointsAdmin = JsonConvert.SerializeObject(dataPointsAdmin);
                //stack chart salesnum\quantity
                List<DataPoint> dataPointssalesNum = new List<DataPoint>();
                List<DataPoint> dataPointsQuantity = new List<DataPoint>();
                getStackGraphData2(false, dataPointssalesNum);
                getStackGraphData2(true, dataPointsQuantity);
                ViewBag.DataPointssalesNum = JsonConvert.SerializeObject(dataPointssalesNum);
                ViewBag.DataPointsQuantity = JsonConvert.SerializeObject(dataPointsQuantity);


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
        public void getStackGraphData(bool isAdmin , List<DataPoint> list)
        {
            DateTime iterateDate = DateTime.Now.AddDays(-30);
            if (isAdmin)
            {
                var result = _unitOfWork.Order.GetAll().Where(a => a.IsAdmin && a.OrderStatus != SD.OrderStatusCancelled).GroupBy(a => a.UsDate)
                          .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).ToList();
                while (iterateDate <= DateTime.Now)
                {
                    if (result.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                              result.Find(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
                    }
                    else
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                    }
                    iterateDate = iterateDate.AddDays(1);
                }
            }
            else
            {
                var result = _unitOfWork.Order.GetAll().Where(a => !a.IsAdmin && a.OrderStatus != SD.OrderStatusCancelled).GroupBy(a => a.UsDate)
                          .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).ToList();
                while (iterateDate <= DateTime.Now)
                {
                    if (result.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                              result.Find(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
                    }
                    else
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                    }
                    iterateDate = iterateDate.AddDays(1);
                }
            }
        }
        //true quantity
        //false salesnum
        public void getStackGraphData2(bool isQuantity, List<DataPoint> list)
        {
            DateTime iterateDate = DateTime.Now.AddDays(-30);
            if (isQuantity)
            {
                var result = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus != SD.OrderStatusCancelled).GroupBy(a => a.UsDate)
                          .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).ToList();
                while (iterateDate <= DateTime.Now)
                {
                    if (result.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                              result.Find(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
                    }
                    else
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                    }
                    iterateDate = iterateDate.AddDays(1);
                }
            }
            else
            {
                var result = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus != SD.OrderStatusCancelled).GroupBy(a => a.UsDate)
                    .Select(g => new { date = g.Key, total = g.Count() }).ToList();
                while (iterateDate <= DateTime.Now)
                {
                    if (result.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                              result.Find(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
                    }
                    else
                    {
                        list.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                    }
                    iterateDate = iterateDate.AddDays(1);
                }
            }
        }
    }
}
