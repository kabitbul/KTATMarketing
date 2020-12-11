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
using Newtonsoft.Json;

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
            return View();
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
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }
        [HttpPost]
        public ActionResult GetList()
        {
            //Server Side parameters
            int start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            int length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            string searchValue = Request.Form["search[value]"].FirstOrDefault();
            string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"].FirstOrDefault();
            string sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            List<ProductInventory> productList = new List<ProductInventory>();
           
            var allWeekOrders = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus != SD.OrderStatusCancelled &&
            a.UsDate >= DateTime.Now.AddDays(-7).Date && a.UsDate <= DateTime.Now.AddDays(-1).Date).OrderBy(a=>a.UsDate);
            initialize(productList);
            int totalRows = productList.Count;
            foreach (Order order in allWeekOrders)
            {
                ProductInventory prodInv = new ProductInventory();
                if (productList.Find(a => a.ProductId == order.ProductId) != null)
                {
                    int val7 = productList.Find(a => a.ProductId == order.ProductId).TotalSales7;
                    productList.Find(a => a.ProductId == order.ProductId).TotalSales7 = val7 + order.Quantity;
                }
                if(order.UsDate >= DateTime.Now.AddDays(-3).Date)
                {
                    int val3 = productList.Find(a => a.ProductId == order.ProductId).TotalSales3;
                    productList.Find(a => a.ProductId == order.ProductId).TotalSales3 = val3+ order.Quantity;
                }
            }
            //calc and populate average
             foreach(ProductInventory prInv in productList)
            {
                prInv.DailyAvg3 = (double)prInv.TotalSales3 / 3;
                prInv.DailyAvg7 = (double)prInv.TotalSales7 / 7;
                if (prInv.DailyAvg3 > 0)
                {
                    prInv.DaysToOOS = (double)(prInv.Inventory + prInv.OnTheWay) / prInv.DailyAvg3;
                }
                if (prInv.DaysToOOS != 0)
                    prInv.DaysToOOSstr = prInv.DaysToOOS.ToString("0.00");
                else
                    prInv.DaysToOOSstr = "0";

                if (prInv.DailyAvg3 != 0)
                    prInv.DailyAvg3str = prInv.DailyAvg3.ToString("0.00");
                else
                    prInv.DailyAvg3str = "0";
                if (prInv.DailyAvg7 != 0)
                    prInv.DailyAvg7str = prInv.DailyAvg7.ToString("0.00");
                else
                    prInv.DailyAvg7str = "0";
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                productList = productList.Where(x => x.ProductName.ToLower().Contains(searchValue.ToLower()) ||
                                            x.Inventory.ToString().Contains(searchValue.ToLower()) ||
                                            x.OnTheWay.ToString().Contains(searchValue.ToLower()) ||
                                            x.Cost.ToString().Contains(searchValue.ToLower()) ||
                                            x.OwnByWarehouse.ToLower().Contains(searchValue.ToLower()) ||
                                            x.Restock.ToLower().Contains(searchValue.ToLower()) ||
                                            x.DailyAvg3.ToString().Contains(searchValue.ToLower()) ||
                                            x.DailyAvg7.ToString().Contains(searchValue.ToLower()) ||
                                            x.DaysToOOS.ToString().Contains(searchValue.ToLower())
                ).ToList<ProductInventory>();
            }
            int totalRowsAfterFiltering = productList.Count;
            if (sortDirection == "desc")
            {
                if (sortColumnName.ToLower() == "productname")
                {
                    productList = productList.OrderByDescending(x => x.ProductName).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "inventory")
                {
                    productList = productList.OrderByDescending(x => x.Inventory).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "ontheway")
                {
                    productList = productList.OrderByDescending(x => x.OnTheWay).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "cost")
                {
                    productList = productList.OrderByDescending(x => x.Cost).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "ownbywarehouse")
                {
                    productList = productList.OrderByDescending(x => x.OwnByWarehouse).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "restock")
                {
                    productList = productList.OrderByDescending(x => x.Restock).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "dailyavg3")
                {
                    productList = productList.OrderByDescending(x => x.DailyAvg3).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "dailyavg7")
                {
                    productList = productList.OrderByDescending(x => x.DailyAvg7).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "daystooosstr")
                {
                    productList = productList.OrderByDescending(x => x.DaysToOOS).ToList<ProductInventory>();
                }
            }
            else
            {
                if (sortColumnName.ToLower() == "productname")
                {
                    productList = productList.OrderBy(x => x.ProductName).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "inventory")
                {
                    productList = productList.OrderBy(x => x.Inventory).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "ontheway")
                {
                    productList = productList.OrderBy(x => x.OnTheWay).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "cost")
                {
                    productList = productList.OrderBy(x => x.Cost).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "ownbywarehouse")
                {
                    productList = productList.OrderBy(x => x.OwnByWarehouse).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "restock")
                {
                    productList = productList.OrderBy(x => x.Restock).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "dailyavg3")
                {
                    productList = productList.OrderBy(x => x.DailyAvg3).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "dailyavg7")
                {
                    productList = productList.OrderBy(x => x.DailyAvg7).ToList<ProductInventory>();
                }
                else if (sortColumnName.ToLower() == "daystooosstr")
                {
                    productList = productList.OrderBy(x => x.DaysToOOS).ToList<ProductInventory>();
                }
            }
            productList = productList.Skip(start).Take(length).ToList<ProductInventory>();
            return Json(new
            {
                data = productList,
                draw = Request.Form["draw"],
                recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFiltering
            });
        }
        public void initialize(List<ProductInventory> productInvList)
        {
            List<Product> allProducts = _unitOfWork.Product.GetAll().ToList();
            
            foreach (Product product in allProducts)
            {
                ProductInventory prodInv = new ProductInventory();
                prodInv.ProductId = product.Id;
                prodInv.ImageUrl = product.ImageUrl;
                prodInv.ProductName = product.ProductName;
                prodInv.Inventory = product.InventoryCount;
                prodInv.OnTheWay = product.OnTheWayInventory;
                prodInv.Cost = product.Cost;
                if(product.OwnByWarehouse)
                {
                    prodInv.OwnByWarehouse = "Yes";
                }
                else
                {
                    prodInv.OwnByWarehouse = "No";
                }
                if (product.ReStock)
                {
                    prodInv.Restock = "Yes";
                }
                else
                {
                    prodInv.Restock = "No";
                }
                prodInv.TotalSales3 = 0;
                prodInv.TotalSales7 = 0;
                prodInv.DailyAvg3 = 0;
                prodInv.DailyAvg7 = 0;
                prodInv.DaysToOOS = 0;
                productInvList.Add(prodInv);
            }
        }
            public void getStackGraphData(bool isAdmin, List<DataPoint> list, int ProductId)
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
