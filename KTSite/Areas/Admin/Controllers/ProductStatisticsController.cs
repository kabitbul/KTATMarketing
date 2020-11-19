using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductStatisticsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductStatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName);
            List<ProductStatistics> prodStatList = new List<ProductStatistics>();
            foreach(Product product in productList)
            {
                ProductStatistics prodStat = new ProductStatistics();
                prodStat.ProductId = product.Id;
                prodStat.SevenDays = getOrdersQuantityByDay(DateTime.Now.AddDays(-7), product.Id);
                prodStat.SixDays = getOrdersQuantityByDay(DateTime.Now.AddDays(-6), product.Id);
                prodStat.FiveDays = getOrdersQuantityByDay(DateTime.Now.AddDays(-5), product.Id);
                prodStat.FourDays = getOrdersQuantityByDay(DateTime.Now.AddDays(-4), product.Id);
                prodStat.ThreeDays = getOrdersQuantityByDay(DateTime.Now.AddDays(-3), product.Id);
                prodStat.TwoDays = getOrdersQuantityByDay(DateTime.Now.AddDays(-2), product.Id);
                prodStat.Yesterday = getOrdersQuantityByDay(DateTime.Now.AddDays(-1), product.Id);
                prodStat.Today = getOrdersQuantityByDay(DateTime.Now, product.Id);
                prodStat.WeeklyAverage = (Convert.ToDouble(prodStat.SevenDays + prodStat.SixDays + prodStat.FiveDays + prodStat.FourDays
                    + prodStat.ThreeDays + prodStat.TwoDays + prodStat.Yesterday)) / 7.0;
                prodStatList.Add(prodStat);
            }
            ViewBag.getProductName = new Func<int, string>(returnProductName);
            return View(prodStatList);
        }
        public int getOrdersQuantityByDay(DateTime date, int prodId)
        {
            return
            _unitOfWork.Order.GetAll().Where(a => a.ProductId == prodId && a.OrderStatus != SD.OrderStatusCancelled && a.UsDate.Date == date.Date).
                  Sum(a => a.Quantity);
        }
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }
        public IActionResult productGraph(int Id)
        {
            //stack chart
            List<DataPoint> dataPointsUser = new List<DataPoint>();
            List<DataPoint> dataPointsAdmin = new List<DataPoint>();
            getStackGraphData(false, dataPointsUser, Id);
            getStackGraphData(true, dataPointsAdmin, Id);
            ViewBag.DataPointsUser = JsonConvert.SerializeObject(dataPointsUser);
            ViewBag.DataPointsAdmin = JsonConvert.SerializeObject(dataPointsAdmin);
            ViewBag.ProductName = returnProductName(Id);
            return View();
        }
        public void getStackGraphData(bool isAdmin,List<DataPoint> list, int ProductId)
        {
            DateTime iterateDate = DateTime.Now.AddDays(-60);
            if (isAdmin)
            {
                var result = _unitOfWork.Order.GetAll().Where(a => a.IsAdmin && a.OrderStatus != SD.OrderStatusCancelled && a.ProductId == ProductId).
                    GroupBy(a => a.UsDate)
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
                var result = _unitOfWork.Order.GetAll().Where(a => !a.IsAdmin && a.OrderStatus != SD.OrderStatusCancelled && a.ProductId == ProductId).
                    GroupBy(a => a.UsDate)
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
        #region API CALLS       
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includePoperties:"Category");
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
