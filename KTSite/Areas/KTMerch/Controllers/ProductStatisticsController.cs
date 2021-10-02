using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using Newtonsoft.Json;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class ProductStatisticsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductStatisticsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {   
            return View();
        }
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }
        public IActionResult productGraph(int Id)
        {
            //stack chart
            List<DataPoint> dataPointsUser = new List<DataPoint>();
            getStackGraphData(dataPointsUser, Id);
            ViewBag.DataPointsUser = JsonConvert.SerializeObject(dataPointsUser);
            ViewBag.ProductName = returnProductName(Id);
            return View();
        }
        [HttpPost]
        public ActionResult GetList()
        {
            string userNameId = returnUserNameId();
            //Server Side parameters
            int start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            int length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            string searchValue = Request.Form["search[value]"].FirstOrDefault();
            string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"].FirstOrDefault();
            string sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            List<ProductStatistics> productList = new List<ProductStatistics>();

            var allWeekOrders = _unitOfWork.Order.GetAll().Where(a =>a.MerchId == userNameId && a.OrderStatus != SD.OrderStatusCancelled &&
            a.UsDate >= DateTime.Now.AddDays(-8)).OrderBy(a => a.UsDate);
            initialize(productList);
            int totalRows = productList.Count;
            foreach (Order order in allWeekOrders)
            {
                ProductStatistics prodInv = new ProductStatistics();
                if (productList.Find(a => a.ProductId == order.ProductId) != null)
                {
                    if (order.UsDate != DateTime.Now.Date)
                    {
                        int val7 = productList.Find(a => a.ProductId == order.ProductId).TotalSales7;
                        productList.Find(a => a.ProductId == order.ProductId).TotalSales7 = val7 + order.Quantity;
                    }
                    /////////ADD QUANTITY TO THE RIGHT DAY//////////////////////////////
                    if(order.UsDate == DateTime.Now.AddDays(-7).Date)
                    {
                        int seven = productList.Find(a => a.ProductId == order.ProductId).SevenDays;
                        productList.Find(a => a.ProductId == order.ProductId).SevenDays = seven + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-6).Date)
                    {
                        int six = productList.Find(a => a.ProductId == order.ProductId).SixDays;
                        productList.Find(a => a.ProductId == order.ProductId).SixDays = six + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-5).Date)
                    {
                        int five = productList.Find(a => a.ProductId == order.ProductId).FiveDays;
                        productList.Find(a => a.ProductId == order.ProductId).FiveDays = five + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-4).Date)
                    {
                        int four = productList.Find(a => a.ProductId == order.ProductId).FourDays;
                        productList.Find(a => a.ProductId == order.ProductId).FourDays = four + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-3).Date)
                    {
                        int three = productList.Find(a => a.ProductId == order.ProductId).ThreeDays;
                        productList.Find(a => a.ProductId == order.ProductId).ThreeDays = three + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-2).Date)
                    {
                        int two = productList.Find(a => a.ProductId == order.ProductId).TwoDays;
                        productList.Find(a => a.ProductId == order.ProductId).TwoDays = two + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-1).Date)
                    {
                        int yesterday = productList.Find(a => a.ProductId == order.ProductId).Yesterday;
                        productList.Find(a => a.ProductId == order.ProductId).Yesterday = yesterday + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.Date)
                    {
                        int today = productList.Find(a => a.ProductId == order.ProductId).Today;
                        productList.Find(a => a.ProductId == order.ProductId).Today = today + order.Quantity;
                    }
                }
            }
            //calc and populate average
            foreach (ProductStatistics prInv in productList)
            {
                prInv.WeeklyAverage = (double)prInv.TotalSales7 / 7;
                prInv.WeeklyAveragestr = prInv.WeeklyAverage.ToString("0.00");
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                productList = productList.Where(x => x.ProductName.ToLower().Contains(searchValue.ToLower()) ||
                                            x.SevenDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.SixDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.FiveDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.FourDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.ThreeDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.TwoDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.Yesterday.ToString().Contains(searchValue.ToLower()) ||
                                            x.Today.ToString().Contains(searchValue.ToLower()) ||
                                            x.WeeklyAveragestr.Contains(searchValue.ToLower())
                ).ToList<ProductStatistics>();
            }
            int totalRowsAfterFiltering = productList.Count;
            if (sortDirection == "desc")
            {
                if (sortColumnName.ToLower() == "productname")
                {
                    productList = productList.OrderByDescending(x => x.ProductName).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "sevendays")
                {
                    productList = productList.OrderByDescending(x => x.SevenDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "sixdays")
                {
                    productList = productList.OrderByDescending(x => x.SixDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "fivedays")
                {
                    productList = productList.OrderByDescending(x => x.FiveDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "fourdays")
                {
                    productList = productList.OrderByDescending(x => x.FourDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "threedays")
                {
                    productList = productList.OrderByDescending(x => x.ThreeDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "twodays")
                {
                    productList = productList.OrderByDescending(x => x.TwoDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "yesterday")
                {
                    productList = productList.OrderByDescending(x => x.Yesterday).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "today")
                {
                    productList = productList.OrderByDescending(x => x.Today).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "weeklyaveragestr")
                {
                    productList = productList.OrderByDescending(x => x.WeeklyAverage).ToList<ProductStatistics>();
                }
            }
            else
            {
                if (sortColumnName.ToLower() == "productname")
                {
                    productList = productList.OrderBy(x => x.ProductName).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "sevendays")
                {
                    productList = productList.OrderBy(x => x.SevenDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "sixdays")
                {
                    productList = productList.OrderBy(x => x.SixDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "fivedays")
                {
                    productList = productList.OrderBy(x => x.FiveDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "fourdays")
                {
                    productList = productList.OrderBy(x => x.FourDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "threedays")
                {
                    productList = productList.OrderBy(x => x.ThreeDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "twodays")
                {
                    productList = productList.OrderBy(x => x.TwoDays).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "yesterday")
                {
                    productList = productList.OrderBy(x => x.Yesterday).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "today")
                {
                    productList = productList.OrderBy(x => x.Today).ToList<ProductStatistics>();
                }
                else if (sortColumnName.ToLower() == "weeklyaveragestr")
                {
                    productList = productList.OrderBy(x => x.WeeklyAverage).ToList<ProductStatistics>();
                }
            }
            productList = productList.Skip(start).Take(length).ToList<ProductStatistics>();
            return Json(new
            {
                data = productList,
                draw = Request.Form["draw"],
                recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFiltering
            });
        }
        public void initialize(List<ProductStatistics> productInvList)
        {
            string userNameId = returnUserNameId();
            List<Product> allProducts = _unitOfWork.Product.GetAll().Where(a=> a.MerchId == userNameId).ToList();

            foreach (Product product in allProducts)
            {
                ProductStatistics prodInv = new ProductStatistics();
                prodInv.ProductId = product.Id;
                prodInv.ImageUrl = product.ImageUrl;
                prodInv.ProductName = product.ProductName;
                prodInv.SevenDays = 0;
                prodInv.SixDays = 0;
                prodInv.FiveDays = 0;
                prodInv.FourDays = 0;
                prodInv.ThreeDays = 0;
                prodInv.TwoDays = 0;
                prodInv.Yesterday = 0;
                prodInv.Today = 0;
                prodInv.WeeklyAverage = 0;
                prodInv.TotalSales7 = 0;
                productInvList.Add(prodInv);
            }
        }
        public void getStackGraphData(List<DataPoint> list, int ProductId)
        {
            DateTime iterateDate = DateTime.Now.AddDays(-60);
                var result = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus != SD.OrderStatusCancelled && a.ProductId == ProductId).
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
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
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
