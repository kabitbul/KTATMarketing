using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Hosting;
using KTSite.DataAccess.Data;

namespace KTSite.Areas.ExternalMerch.Controllers
{
    [Area("ExternalMerch")]
    [Authorize(Roles = SD.Role_ExMerch)]
    [EnableCors(origins: "https://ktatmarketing.azurewebsites.net", headers: "*", methods: "*")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _db;
        public OrderController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _db = db;
        }
        public IActionResult Index()
        {
            dynamic myModel = new System.Dynamic.ExpandoObject();
            //myModel.ordercsv = new List<KTSite.Areas.Warehouse.Views.OrderWarehouse.CSVOrderLine>();
            //ViewBag.getProductName =
             //  new Func<int, string>(returnProductName);
            //ViewBag.getStoreName =
             //  new Func<int, string>(returnStoreName);
            //ViewBag.getCost =
              //            new Func<int, double, double>(returnCost);
            //ViewBag.errSaveInProgress = false;
            //ViewBag.ExistInProgress = false;
            //ViewBag.NoOrdersMsg = false;
            return View(myModel);
        }
        //public void setInProgressStatus(long Id)
        //{
        //    _unitOfWork.Order.Get(Id).OrderStatus = SD.OrderStatusInProgress;
        //    //Order order = _unitOfWork.Order.Get(Id);
        //    //order.OrderStatus = SD.OrderStatusInProgress;
        //    //_unitOfWork.Order.update(order);
        //    _unitOfWork.Save();
        //}
        public string getProductName(int productId)
        {
            return _unitOfWork.Product.Get(productId).ProductName;
        }
        public string getWeight(int productId, int Quantity)
        {
            return (_unitOfWork.Product.Get(productId).Weight*Quantity).ToString();
        }
        public IActionResult AddTrackingManually(long id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            OrderVM orderVM = new OrderVM()
            {

                Orders = new Order()

            };

            orderVM.Orders = _unitOfWork.Order.Get(id);
            if (orderVM.Orders == null)
            {
                return NotFound();
            }
            ViewBag.emptyTracking = false;
            return View(orderVM);
        }
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }
        public double returnCost(int productId, double quantity)
        {
            double productCost = (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.Cost)).FirstOrDefault();
            return Convert.ToDouble(String.Format("{0:0.00}", (productCost * quantity)));
        }

        [HttpPost]
        public JsonResult ChangeInProgress()
        {
            var orderList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusInProgress);
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (Order order in orderList)
                    {
                        order.OrderStatus = SD.OrderStatusAccepted;
                        //_unitOfWork.Order.update(order);
                        _db.Orders.Update(order);
                    }
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch
                { }
            }
                return Json(new { });
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
            List<Order> orderList = new List<Order>();
            string uNameId = returnUserNameId();
            orderList = _unitOfWork.Order.GetAll().Where(a=>a.MerchType == SD.Role_ExMerch && a.MerchId == uNameId).ToList();
            int totalRows = orderList.Count;
            foreach (Order order in orderList)
            {
                order.StringDate = order.UsDate.Day + "/" + order.UsDate.Month + "/" + order.UsDate.Year;
                order.FullAddress = order.CustName + "\n" + order.CustStreet1 + "\n";
                if (!string.IsNullOrEmpty(order.CustStreet2))
                {
                    order.FullAddress = order.FullAddress + order.CustStreet2 + "\n";
                }
                order.FullAddress = order.FullAddress + order.CustCity + " " + order.CustState + " " + order.CustZipCode + "\n"
                                                                                        + "United States";
                if (order.CustPhone != null && order.CustPhone.Length > 0)
                {
                    order.FullAddress = order.FullAddress + "\n" + order.CustPhone;
                }
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                orderList = orderList.Where(x => x.FullAddress.ToLower().Contains(searchValue.ToLower()) ||
                                            x.Id.ToString().Contains(searchValue.ToLower()) ||
                                            x.OrderStatus.ToLower().Contains(searchValue.ToLower()) ||
                                            x.ProductName.ToLower().Contains(searchValue.ToLower()) ||
                                            x.StringDate.ToLower().Contains(searchValue.ToLower()) ||
                                            x.Quantity.ToString().Contains(searchValue.ToLower()) ||
                                            (!string.IsNullOrEmpty(x.TrackingNumber) && x.TrackingNumber.ToString().Contains(searchValue.ToLower()))
                ).ToList<Order>();
            }
            int totalRowsAfterFiltering = orderList.Count;
            //Handle Sorting
            if (sortDirection == "desc")
            {
                if (sortColumnName.ToLower() == "id")
                {
                    orderList = orderList.OrderByDescending(x => x.Id).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "orderstatus")
                {
                    orderList = orderList.OrderByDescending(x => x.OrderStatus).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "productname")
                {
                    orderList = orderList.OrderByDescending(x => x.ProductName).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "stringdate")
                {
                    orderList = orderList.OrderByDescending(x => x.UsDate).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "quantity")
                {
                    orderList = orderList.OrderByDescending(x => x.Quantity).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "fulladdress")
                {
                    orderList = orderList.OrderByDescending(x => x.FullAddress).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "trackingnumber")
                {
                    orderList = orderList.OrderByDescending(x => x.TrackingNumber).ToList<Order>();
                }
            }
            else
            {
                if (sortColumnName.ToLower() == "id")
                {
                    orderList = orderList.OrderBy(x => x.Id).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "orderstatus")
                {
                    orderList = orderList.OrderBy(x => x.OrderStatus).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "productname")
                {
                    orderList = orderList.OrderBy(x => x.ProductName).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "stringdate")
                {
                    orderList = orderList.OrderBy(x => x.UsDate).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "quantity")
                {
                    orderList = orderList.OrderBy(x => x.Quantity).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "fulladdress")
                {
                    orderList = orderList.OrderBy(x => x.FullAddress).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "trackingnumber")
                {
                    orderList = orderList.OrderBy(x => x.TrackingNumber).ToList<Order>();
                }
            }
            //orderList = orderList.OrderBy(x => x.Id //sortColumnName + " " + sortDirection).ToList<Order>();
            orderList = orderList.Skip(start).Take(length).ToList<Order>();
            return Json(new
            {
                data = orderList,
                draw = Request.Form["draw"],
                recordsTotal = totalRows,
                recordsFiltered = totalRowsAfterFiltering
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTrackingManually(OrderVM orderVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                // if(isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now)
                //            {
                if (orderVM.Orders.TrackingNumber == null)
                {
                    ViewBag.emptyTracking = true;
                }
                else
                {
                    orderVM.Orders.OrderStatus = SD.OrderStatusDone;
                    orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                    //orderVM.Orders.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                    //orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                    _unitOfWork.Order.update(orderVM.Orders);
                    _unitOfWork.Save();
                    ViewBag.success = true;
                    ViewBag.failed = false;
                    ViewBag.emptyTracking = false;
                }
            }
            return View(orderVM);
        }

        public bool isStoreAuthenticated(OrderVM orderVM)
        {
            //get userName id based on store
            bool isAdmin = _unitOfWork.UserStoreName.GetAll().Where
                (a => a.Id == orderVM.Orders.StoreNameId).
                Select(a => a.IsAdminStore).FirstOrDefault();
            if (isAdmin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        public int getProductIdByName(string productName)
        {
            return _unitOfWork.Product.GetAll().Where(a => a.ProductName.Equals(productName, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id).FirstOrDefault();
        }
        public int getStoreNameId(string storeName)
        {
            return _unitOfWork.UserStoreName.GetAll().Where(a => a.StoreName.Equals(storeName, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id).FirstOrDefault();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Order.GetAll();
            return Json(new { data = allObj });
        }
        [HttpGet]
        public IActionResult GetAllWithoutTracking()
        {
            var allObj = _unitOfWork.Order.GetAll().Where(a => a.TrackingNumber == null);
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if (objFromDb == null)
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
