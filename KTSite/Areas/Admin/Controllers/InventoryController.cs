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

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class InventoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public InventoryController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> product = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName);
            ViewBag.getDailyAvg =  new Func<int, int,double>(returnDailyAvg);
            ViewBag.getDaysToOOS = new Func<int, double>(returnDaysToOOS);
            ViewBag.getPaintData = new Func<int, bool>(returnPaintData);
            return View(product);
        }
        public bool returnPaintData(int productId)
        {
            double daysToOOS = returnDaysToOOS(productId);
            if (daysToOOS <= 100 && daysToOOS > 0)
                return true;
            return false;
        }
        public double returnDaysToOOS(int productId)
        {
            Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            double avgsells = returnDailyAvg(productId, 3);
            if(avgsells == 0)
            {
                return 999999.99;
            }
            return Convert.ToDouble(((product.InventoryCount + product.OnTheWayInventory) / avgsells).ToString("0.00"));
        }
        public double returnDailyAvg(int productId, int avgDay)
        {
            int Count = _unitOfWork.Order.GetAll().Where(a => a.ProductId == productId &&
                              a.OrderStatus != SD.OrderStatusCancelled &&
                              a.UsDate.Date >= DateTime.Now.AddDays((avgDay * (-1))).Date &&
                              a.UsDate.Date <= DateTime.Now.AddDays(-1).Date).Sum(a => a.Quantity);
            if (Count > 0)
            {
                return (double)Count / avgDay;
            }
            return 0;
        }
        public string getCategoryName(int CategoryId)
        {
            return _unitOfWork.Category.GetAll().Where(a => a.Id == CategoryId).Select(a => a.Name).FirstOrDefault();
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
