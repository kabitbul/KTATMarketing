using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            ViewBag.ExistProgress = _unitOfWork.Order.GetAll().Any(a => a.OrderStatus == SD.OrderStatusInProgress && a.MerchType != SD.Role_ExMerch);
            int WaitingForProcess = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusAccepted && a.MerchType != SD.Role_ExMerch).Count();
            ViewBag.WaitingForProcess = WaitingForProcess;
            int WaitingForReturnLabel = _unitOfWork.ReturnLabel.GetAll().Where(a => a.FileURL == null && a.MerchType != SD.Role_ExMerch).Count();
            int missingWeightCount = _unitOfWork.Product.GetAll().Where(a => a.MerchType != SD.Role_ExMerch && a.Weight == 0 && (a.InventoryCount > 0 || 
            _unitOfWork.ArrivingFromChina.GetAll().Where(b => b.ProductId == a.Id).Count()>0 )).Count();
            ViewBag.missingWeightCount = missingWeightCount;
            ViewBag.WaitingForReturnLabel = WaitingForReturnLabel;
            DateTime iterateDate = DateTime.Now.AddDays(-30);
                List<DataPoint> dataPointsKT = new List<DataPoint>();
                List<DataPoint> dataPointsWarehouse = new List<DataPoint>();
            var resultWarehouse = _unitOfWork.Order.GetAll().Where(a=>a.OrderStatus != SD.OrderStatusCancelled && a.MerchType != SD.Role_ExMerch
            && isWarehouse(a.ProductId)
            ).GroupBy(a => a.UsDate)
                       .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).ToList();
            var resultNotWarehouse = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus != SD.OrderStatusCancelled && a.MerchType != SD.Role_ExMerch
            && !isWarehouse(a.ProductId)
            ).GroupBy(a => a.UsDate)
                       .Select(g => new { date = g.Key, total = g.Sum(i => i.Quantity) }).ToList();
            while (iterateDate <= DateTime.Now)
                {
                  if (resultWarehouse.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                  {
                    dataPointsWarehouse.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                          resultWarehouse.Find(x=> x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
                  }
                  else
                {
                    dataPointsWarehouse.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                }
                if (resultNotWarehouse.Exists(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")))
                {
                    dataPointsKT.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(),
                                          resultNotWarehouse.Find(x => x.date.ToString("dd/MM") == iterateDate.ToString("dd/MM")).total));
                }
                else
                {
                    dataPointsKT.Add(new DataPoint(iterateDate.Day.ToString() + "/" + iterateDate.Month.ToString(), 0));
                }
                iterateDate = iterateDate.AddDays(1);
                }
            //exist complaints unsolved in warehouse responsibility
            ViewBag.NumOfComplaints =_unitOfWork.Complaints.GetAll().Where(a => a.WarehouseResponsibility && !a.Solved && a.MerchType != SD.Role_ExMerch ).Count();
            ViewBag.NeedCounting = _unitOfWork.ArrivingFromChina.GetAll().Where(a => !a.UpdatedByAdmin && a.Quantity == 0).Count();
                ViewBag.DataPointsKT = JsonConvert.SerializeObject(dataPointsKT);
                ViewBag.DataPointsWarehouse = JsonConvert.SerializeObject(dataPointsWarehouse);
            return View();
            }
        public bool isWarehouse(int id)
        {
            return _unitOfWork.Product.Get(id).OwnByWarehouse;
        }
        [HttpPost]
        public JsonResult ShowSummary(DateTime fromDate, DateTime toDate)
        {
            var orderList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus != SD.OrderStatusCancelled &&
             a.OrderStatus != SD.OrderStatusAccepted &&
             a.UsDate >= fromDate && a.UsDate <= toDate);

            List<string> strRes = new List<string>();
            strRes.Add("Total Orders Shipped: " + orderList.Count());
            strRes.Add("Total Products sold: " + orderList.Sum(a=> a.Quantity));
            strRes.Add("Total Shipping Cost(not Accurate, By 4$ for all items): " + (orderList.Sum(a => a.Quantity) * SD.shipping_cost).ToString("0.00")+ "$");
            var prod = _unitOfWork.Product.GetAll().Where(a=>a.OwnByWarehouse);
            double totalWarehouseProfit = 0.0;
            int prodSum = 0;
            strRes.Add("----------------------");
            foreach (Product pr in prod)
            {
                prodSum = orderList.Where(a => a.ProductId == pr.Id).Sum(a => a.Quantity);
                strRes.Add(pr.ProductName + " Sold: " + prodSum);
                strRes.Add("Profit:" + ((pr.Cost - pr.WarehouseChinaCost) * prodSum).ToString("0.00") +"$");
                strRes.Add("----------------------");
                totalWarehouseProfit = totalWarehouseProfit + ((pr.Cost - pr.WarehouseChinaCost) * prodSum);
            }
            if(totalWarehouseProfit > 0 )
            {
                strRes.Add("Total Profit From products Own by warehouse is: " + totalWarehouseProfit.ToString("0.00") + "$");
            }
            return Json(strRes);
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
