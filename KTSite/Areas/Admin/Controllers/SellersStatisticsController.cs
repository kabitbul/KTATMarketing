using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using Newtonsoft.Json;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class SellersStatisticsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SellersStatisticsController(IUnitOfWork unitOfWork)
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
        public IActionResult sellersGraph(string Id)
        {
            //stack chart
            List<DataPoint> dataPointsUser = new List<DataPoint>();
            getGraphData(dataPointsUser, Id);
            ViewBag.DataPointsUser = JsonConvert.SerializeObject(dataPointsUser);
            ViewBag.UName = _unitOfWork.ApplicationUser.Get(Id).Name;
            return View();
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
            List<SellersStatistics> sellersList = new List<SellersStatistics>();

            var allWeekOrders = _unitOfWork.Order.GetAll().Where(a => !a.IsAdmin &&a.OrderStatus != SD.OrderStatusCancelled &&
            a.UsDate >= DateTime.Now.AddDays(-8)).OrderBy(a => a.UsDate);
            initialize(sellersList);
            int totalRows = sellersList.Count;
            foreach (Order order in allWeekOrders)
            {
                SellersStatistics prodInv = new SellersStatistics();
                if (sellersList.Find(a => a.UserNameId == order.UserNameId) != null)
                {
                    if (order.UsDate != DateTime.Now.Date)
                    {
                        int val7 = sellersList.Find(a => a.UserNameId == order.UserNameId).TotalSales7;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).TotalSales7 = val7 + order.Quantity;
                    }
                    /////////ADD QUANTITY TO THE RIGHT DAY//////////////////////////////
                    if(order.UsDate == DateTime.Now.AddDays(-7).Date)
                    {
                        int seven = sellersList.Find(a => a.UserNameId == order.UserNameId).SevenDays;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).SevenDays = seven + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-6).Date)
                    {
                        int six = sellersList.Find(a => a.UserNameId == order.UserNameId).SixDays;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).SixDays = six + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-5).Date)
                    {
                        int five = sellersList.Find(a => a.UserNameId == order.UserNameId).FiveDays;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).FiveDays = five + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-4).Date)
                    {
                        int four = sellersList.Find(a => a.UserNameId == order.UserNameId).FourDays;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).FourDays = four + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-3).Date)
                    {
                        int three = sellersList.Find(a => a.UserNameId == order.UserNameId).ThreeDays;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).ThreeDays = three + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-2).Date)
                    {
                        int two = sellersList.Find(a => a.UserNameId == order.UserNameId).TwoDays;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).TwoDays = two + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.AddDays(-1).Date)
                    {
                        int yesterday = sellersList.Find(a => a.UserNameId == order.UserNameId).Yesterday;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).Yesterday = yesterday + order.Quantity;
                    }
                    else if (order.UsDate == DateTime.Now.Date)
                    {
                        int today = sellersList.Find(a => a.UserNameId == order.UserNameId).Today;
                        sellersList.Find(a => a.UserNameId == order.UserNameId).Today = today + order.Quantity;
                    }
                }
            }
            //calc and populate average
            foreach (SellersStatistics slStat in sellersList)
            {
                slStat.WeeklyAverage = (double)slStat.TotalSales7 / 7;
                slStat.WeeklyAveragestr = slStat.WeeklyAverage.ToString("0.00");
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                sellersList = sellersList.Where(x => x.UserName.ToLower().Contains(searchValue.ToLower()) ||
                                            x.SevenDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.SixDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.FiveDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.FourDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.ThreeDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.TwoDays.ToString().Contains(searchValue.ToLower()) ||
                                            x.Yesterday.ToString().Contains(searchValue.ToLower()) ||
                                            x.Today.ToString().Contains(searchValue.ToLower()) ||
                                            x.WeeklyAveragestr.Contains(searchValue.ToLower())
                ).ToList<SellersStatistics>();
            }
            int totalRowsAfterFiltering = sellersList.Count;
            if (sortDirection == "desc")
            {
                if (sortColumnName.ToLower() == "username")
                {
                    sellersList = sellersList.OrderByDescending(x => x.UserName).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "sevendays")
                {
                    sellersList = sellersList.OrderByDescending(x => x.SevenDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "sixdays")
                {
                    sellersList = sellersList.OrderByDescending(x => x.SixDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "fivedays")
                {
                    sellersList = sellersList.OrderByDescending(x => x.FiveDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "fourdays")
                {
                    sellersList = sellersList.OrderByDescending(x => x.FourDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "threedays")
                {
                    sellersList = sellersList.OrderByDescending(x => x.ThreeDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "twodays")
                {
                    sellersList = sellersList.OrderByDescending(x => x.TwoDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "yesterday")
                {
                    sellersList = sellersList.OrderByDescending(x => x.Yesterday).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "today")
                {
                    sellersList = sellersList.OrderByDescending(x => x.Today).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "weeklyaveragestr")
                {
                    sellersList = sellersList.OrderByDescending(x => x.WeeklyAverage).ToList<SellersStatistics>();
                }
            }
            else
            {
                if (sortColumnName.ToLower() == "username")
                {
                    sellersList = sellersList.OrderBy(x => x.UserName).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "sevendays")
                {
                    sellersList = sellersList.OrderBy(x => x.SevenDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "sixdays")
                {
                    sellersList = sellersList.OrderBy(x => x.SixDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "fivedays")
                {
                    sellersList = sellersList.OrderBy(x => x.FiveDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "fourdays")
                {
                    sellersList = sellersList.OrderBy(x => x.FourDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "threedays")
                {
                    sellersList = sellersList.OrderBy(x => x.ThreeDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "twodays")
                {
                    sellersList = sellersList.OrderBy(x => x.TwoDays).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "yesterday")
                {
                    sellersList = sellersList.OrderBy(x => x.Yesterday).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "today")
                {
                    sellersList = sellersList.OrderBy(x => x.Today).ToList<SellersStatistics>();
                }
                else if (sortColumnName.ToLower() == "weeklyaveragestr")
                {
                    sellersList = sellersList.OrderBy(x => x.WeeklyAverage).ToList<SellersStatistics>();
                }
            }
            sellersList = sellersList.Skip(start).Take(length).ToList<SellersStatistics>();
            return Json(new
            {
                data = sellersList,
                draw = Request.Form["draw"],
                recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFiltering
            });
        }
        public void initialize(List<SellersStatistics> sellersInvList)
        {
            List<ApplicationUser> allUsers = _unitOfWork.ApplicationUser.GetAll().Where(a=>a.Role == SD.Role_Users).ToList();

            foreach (ApplicationUser applicationUser in allUsers)
            {
                SellersStatistics sellerInv = new SellersStatistics();
                sellerInv.UserNameId = applicationUser.Id;
                sellerInv.UserName = applicationUser.Name;
                sellerInv.SevenDays = 0;
                sellerInv.SixDays = 0;
                sellerInv.FiveDays = 0;
                sellerInv.FourDays = 0;
                sellerInv.ThreeDays = 0;
                sellerInv.TwoDays = 0;
                sellerInv.Yesterday = 0;
                sellerInv.Today = 0;
                sellerInv.WeeklyAverage = 0;
                sellerInv.TotalSales7 = 0;
                sellersInvList.Add(sellerInv);
            }
        }
        public void getGraphData(List<DataPoint> list, string UserNameId)
        {
            DateTime iterateDate = DateTime.Now.AddDays(-60);

                var result = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == UserNameId && 
                             a.OrderStatus != SD.OrderStatusCancelled).
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
