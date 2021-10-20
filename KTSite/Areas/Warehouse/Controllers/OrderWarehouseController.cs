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

namespace KTSite.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Authorize(Roles = SD.Role_Warehouse)]
    [EnableCors(origins: "https://ktatmarketing.azurewebsites.net", headers: "*", methods: "*")]
    public class OrderWarehouseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _db;
        public OrderWarehouseController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _db = db;
        }
        public IActionResult Index()
        {
            dynamic myModel = new System.Dynamic.ExpandoObject();
            myModel.ordercsv = new List<KTSite.Areas.Warehouse.Views.OrderWarehouse.CSVOrderLine>();
            ViewBag.getProductName =
               new Func<int, string>(returnProductName);
            ViewBag.getStoreName =
               new Func<int, string>(returnStoreName);
            ViewBag.getCost =
                          new Func<int, double, double>(returnCost);
            ViewBag.errSaveInProgress = false;
            ViewBag.ExistInProgress = false;
            ViewBag.NoOrdersMsg = false;
            return View(myModel);
        }
        public void setInProgressStatus(long Id)
        {
            _unitOfWork.Order.Get(Id).OrderStatus = SD.OrderStatusInProgress;
            //Order order = _unitOfWork.Order.Get(Id);
            //order.OrderStatus = SD.OrderStatusInProgress;
            //_unitOfWork.Order.update(order);
            _unitOfWork.Save();
        }
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
        public string returnStoreName(int storeId)
        {
            return (_unitOfWork.UserStoreName.GetAll().Where(q => q.Id == storeId).Select(q => q.StoreName)).FirstOrDefault();
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
        public ActionResult Export()
        {
            string fileName =
                    DateTime.Now.DayOfWeek + "_HH" + DateTime.Now.Hour + "_MI" + DateTime.Now.Minute + ".csv";
            IEnumerable<Order> orderList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusAccepted).
                //OrderBy(a => getProductName(a.ProductId)).ThenByDescending(a=>a.Quantity).Take(300);
                OrderBy(a => a.ProductId).ThenByDescending(a => a.Quantity).Take(300);
            ViewBag.errSaveInProgress = false;
            int lineCounter = 0;
            bool existInProgress = _unitOfWork.Order.GetAll().Any(a => a.OrderStatus == SD.OrderStatusInProgress);
            if (existInProgress)
            {
                dynamic myModel = new System.Dynamic.ExpandoObject();
                myModel.Order = _unitOfWork.Order.GetAll();
                myModel.ordercsv = new List<KTSite.Areas.Warehouse.Views.OrderWarehouse.CSVOrderLine>();
                ViewBag.getProductName =
                   new Func<int, string>(returnProductName);
                ViewBag.getStoreName =
                   new Func<int, string>(returnStoreName);
                ViewBag.getCost =
                              new Func<int, double, double>(returnCost);
                ViewBag.errSaveInProgress = false;
                ViewBag.ExistInProgress = true;
                ViewBag.NoOrdersMsg = false;
                //throw new Exception(String.Format("exceptStatusNoAcceptedLeft"));
                return View("Index", myModel);
            }
            if (orderList.Count() == 0)
            {
                dynamic myModel = new System.Dynamic.ExpandoObject();
                myModel.Order = _unitOfWork.Order.GetAll();
                myModel.ordercsv = new List<KTSite.Areas.Warehouse.Views.OrderWarehouse.CSVOrderLine>();
                ViewBag.getProductName =
                   new Func<int, string>(returnProductName);
                ViewBag.getStoreName =
                   new Func<int, string>(returnStoreName);
                ViewBag.getCost =
                              new Func<int, double, double>(returnCost);
                ViewBag.errSaveInProgress = false;
                ViewBag.ExistInProgress = false;
                ViewBag.NoOrdersMsg = true;
                //throw new Exception(String.Format("exceptStatusNoAcceptedLeft"));
                return View("Index", myModel);
            }
            //try
            //{
            //}
            //catch (IOException e)
            //{
            //    throw new Exception(String.Format("exceptStatusCantCreateFile"));
            //}
            StringBuilder sb = new StringBuilder();
            //Header
            sb.Append("Product,Empty,Name,Address1,Address2,City,State,zip,Country,Phone,Quantity,Weight");
            sb.Append("\r\n");
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try { 
                foreach (Order order in orderList)
                {
                    if (lineCounter >= 300)
                    {
                        break;
                    }
                    sb.Append("KT " + order.ProductId + "# " +getProductName(order.ProductId) + " - " + order.Quantity + ',');
                    sb.Append(',');
                    sb.Append(order.CustName.Replace(",", "").Replace("\"", "") + ',');
                    sb.Append(order.CustStreet1.Replace(",", "").Replace("\"", "") + ',');
                    if (string.IsNullOrEmpty(order.CustStreet2))
                    {
                        sb.Append(',');
                    }
                    else
                    {
                        sb.Append(order.CustStreet2.Replace(",", "").Replace("\"", "") + ',');
                    }
                    sb.Append(order.CustCity.Replace(",", "") + ',');
                    sb.Append(order.CustState.Replace(",", "") + ',');
                    sb.Append(order.CustZipCode.Replace(",", "") + ',');
                    sb.Append("US" + ',');
                    if (!string.IsNullOrEmpty(order.CustPhone))
                    {
                        sb.Append(order.CustPhone + ',');
                    }
                    else
                    {
                        sb.Append("999-999-9999" + ',');
                    }
                    sb.Append(order.Quantity.ToString() + ',');
                    sb.Append(getWeight(order.ProductId, order.Quantity) + ',');

                            _unitOfWork.Order.Get(order.Id).OrderStatus = SD.OrderStatusInProgress;
                            _db.Orders.Update(_unitOfWork.Order.Get(order.Id));
                        //setInProgressStatus(order.Id);
                    
                    //Append new line character.
                    sb.Append("\r\n");
                    lineCounter++;
                }
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch
            {
                ViewBag.errSaveInProgress = true;
            }
        }
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", fileName);
        }
        [HttpPost]
        public ActionResult ExportNew()
        {
            string fileName =
                    DateTime.Now.DayOfWeek + "_HH" + DateTime.Now.Hour + "_MI" + DateTime.Now.Minute + "_NEW.csv";
            IEnumerable<Order> orderList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusAccepted).
                OrderBy(a => getProductName(a.ProductId)).ThenByDescending(a => a.Quantity).Take(300);
            ViewBag.errSaveInProgress = false;
            int lineCounter = 0;
            bool existInProgress = _unitOfWork.Order.GetAll().Any(a => a.OrderStatus == SD.OrderStatusInProgress);
            if (existInProgress)
            {
                dynamic myModel = new System.Dynamic.ExpandoObject();
                myModel.Order = _unitOfWork.Order.GetAll();
                myModel.ordercsv = new List<KTSite.Areas.Warehouse.Views.OrderWarehouse.CSVOrderLine>();
                ViewBag.getProductName =
                   new Func<int, string>(returnProductName);
                ViewBag.getStoreName =
                   new Func<int, string>(returnStoreName);
                ViewBag.getCost =
                              new Func<int, double, double>(returnCost);
                ViewBag.errSaveInProgress = false;
                ViewBag.ExistInProgress = true;
                ViewBag.NoOrdersMsg = false;
                //throw new Exception(String.Format("exceptStatusNoAcceptedLeft"));
                return View("Index", myModel);
            }
            if (orderList.Count() == 0)
            {
                dynamic myModel = new System.Dynamic.ExpandoObject();
                myModel.Order = _unitOfWork.Order.GetAll();
                myModel.ordercsv = new List<KTSite.Areas.Warehouse.Views.OrderWarehouse.CSVOrderLine>();
                ViewBag.getProductName =
                   new Func<int, string>(returnProductName);
                ViewBag.getStoreName =
                   new Func<int, string>(returnStoreName);
                ViewBag.getCost =
                              new Func<int, double, double>(returnCost);
                ViewBag.errSaveInProgress = false;
                ViewBag.ExistInProgress = false;
                ViewBag.NoOrdersMsg = true;
                //throw new Exception(String.Format("exceptStatusNoAcceptedLeft"));
                return View("Index", myModel);
            }
            StringBuilder sb = new StringBuilder();
            //Header
            sb.Append("OrderID,OrderDate,OrderValue,RequestedService,Name,ShipToCompany,Address1,Address2,Address3" +
                ",State,City,PostalCode,Country,Phone,Email,TotalWeightInOz,Length,Width,Height,Notes,Notes2,GiftWrap,GiftMessage");
            sb.Append("\r\n");
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (Order order in orderList)
                    {
                        if (lineCounter >= 300)
                        {
                            break;
                        }
                        sb.Append(order.Id.ToString() + ',');
                        sb.Append(order.UsDate.Date.ToString("MM/dd/yyyy") + ',');
                        sb.Append("0" + ',');
                        sb.Append(',');
                        sb.Append(order.CustName.Replace(",", "").Replace("\"", "") + ',');
                        sb.Append(',');
                        sb.Append(order.CustStreet1.Replace(",", "").Replace("\"", "") + ',');
                        if (string.IsNullOrEmpty(order.CustStreet2))
                        {
                            sb.Append(',');
                        }
                        else
                        {
                            sb.Append(order.CustStreet2.Replace(",", "").Replace("\"", "") + ',');
                        }
                        sb.Append(',');
                        sb.Append(order.CustState.Replace(",", "") + ',');
                        sb.Append(order.CustCity.Replace(",", "") + ',');
                        sb.Append(order.CustZipCode.Replace(",", "") + ',');
                        sb.Append("US" + ',');
                        if (!string.IsNullOrEmpty(order.CustPhone))
                        {
                            sb.Append(order.CustPhone + ',');
                        }
                        else
                        {
                            sb.Append("999-999-9999" + ',');
                        }
                        sb.Append(',');
                        sb.Append((Convert.ToDouble(getWeight(order.ProductId, order.Quantity))*16).ToString() + ',');
                        sb.Append(',');
                        sb.Append(',');
                        sb.Append(',');
                        sb.Append(getProductName(order.ProductId) + "- " + order.Quantity + ',');
                        sb.Append(',');
                        sb.Append(',');
                        sb.Append(',');

                        _unitOfWork.Order.Get(order.Id).OrderStatus = SD.OrderStatusInProgress;
                        _db.Orders.Update(_unitOfWork.Order.Get(order.Id));
                        //setInProgressStatus(order.Id);

                        //Append new line character.
                        sb.Append("\r\n");
                        lineCounter++;
                    }
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch
                {
                    ViewBag.errSaveInProgress = true;
                }
            }
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", fileName);
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
            orderList = _unitOfWork.Order.GetAll().ToList();
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
                                            x.StoreName.ToLower().Contains(searchValue.ToLower()) ||
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
                else if (sortColumnName.ToLower() == "storename")
                {
                    orderList = orderList.OrderByDescending(x => x.StoreName).ToList<Order>();
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
                else if (sortColumnName.ToLower() == "storename")
                {
                    orderList = orderList.OrderBy(x => x.StoreName).ToList<Order>();
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
        public JsonResult Submit(IFormFile CSVFile)
        {
            int success = 0;
            string excep = "";
            var result = new StringBuilder();
            bool existFail = false;
            int countRec = 0;
            try
            {
                using (var reader = new StreamReader(CSVFile.OpenReadStream()))
                {
                    if (reader.Peek() <= 1)
                    {
                        success = 0;
                        excep = "File is Empty";
                        return Json(new { excep, success });
                    }
                    while (reader.Peek() >= 0)
                        result.AppendLine(reader.ReadLine());
                }
                LogsData log4 = new LogsData();
                log4.Msg1 = "update tracking numbers - start";
                log4.Msg2 = "";
                log4.CreatedBy = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
                log4.CreatedDate = DateTime.Now;
                _unitOfWork.logsData.Add(log4);
                _unitOfWork.Save();
                string[] lines = result.ToString().Split(Environment.NewLine.ToCharArray());
                lines = lines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                var OrdersInProgress = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusInProgress && a.TrackingNumber == null);
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    foreach (String line in lines)
                    {
                        try { 
                        string[] columns = line.Split(',');
                        if (columns[52] != null)
                        {
                            string custName = columns[52].Replace("\"", "");
                            string custZip = columns[58].Replace("\"", "").Replace("=", "");
                                Order order =
                                        OrdersInProgress.Where(a => a.CustName.Replace(",", "").Replace("\"", "") == custName &&
                                                                    a.CustZipCode.Substring(0, 5) == custZip.Substring(0, 5)).FirstOrDefault();
                               //_unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusInProgress)
                                 //                        .Where(a => a.CustName.Replace(",", "").Replace("\"", "") == custName)
                                   //                      .Where(a => a.CustZipCode.Substring(0, 5) == custZip.Substring(0, 5))//compare first 5
                                     //                    .Where(a => a.TrackingNumber == null)
                                       //                  .FirstOrDefault();
                                if (order != null)
                            {
                                order.TrackingNumber = columns[1].Replace("\"", "").Replace("=", "");
                                order.Carrier = columns[2].Replace("\"", "");
                                order.OrderStatus = SD.OrderStatusDone;
                                _db.Orders.Update(order);
                                    countRec++;
                            }
                        }
                    }
                    catch
            {
                            LogsData log2 = new LogsData();
                            existFail = true;
                            string[] columns = line.Split(',');
                            log2.Msg1 = "updateTrackingNumbers failed -- customer: " + columns[52].Replace("\"", "") + " zipCode: " + columns[58].Replace("\"", "").Replace("=", "");
                            log2.Msg2 = "";
                            log2.CreatedBy = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
                            log2.CreatedDate = DateTime.Now;
                            _unitOfWork.logsData.Add(log2);
                            _unitOfWork.Save();

                        }
                    }
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                if (!existFail)
                {
                    success = 1;
                    excep = "Tracking Updated Succesfully!";
                }
                else
                {
                    success = 0;
                    excep = "There was an Error, some orders were not updated!";
                }

            }
            catch
            {
                success = 0;
                LogsData log3 = new LogsData();
                excep = "There was an Error, some orders were not updated!";
                log3.Msg1 = "update tracking numbers - general error orders updated: "+ countRec;
                log3.Msg2 = "";
                log3.CreatedBy = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
                log3.CreatedDate = DateTime.Now;
                _unitOfWork.logsData.Add(log3);
                _unitOfWork.Save();
            }
            LogsData log = new LogsData();
            log.Msg1 = "update tracking numbers - end";
            log.Msg2 = "successfully updated " + countRec + " orders";
            log.CreatedBy = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
            log.CreatedDate = DateTime.Now;
            _unitOfWork.logsData.Add(log);
            _unitOfWork.Save();
            return Json(new { excep, success });
        }
        [HttpPost]
        public JsonResult SubmitNew(IFormFile CSVFileNew)
        {
            int success = 0;
            string excep = "";
            var result = new StringBuilder();
            try
            {
                using (var reader = new StreamReader(CSVFileNew.OpenReadStream()))
                {
                    if (reader.Peek() <= 1)
                    {
                        success = 0;
                        excep = "File is Empty";
                        return Json(new { excep, success });
                    }
                    while (reader.Peek() >= 0)
                        result.AppendLine(reader.ReadLine());
                }
                string[] lines = result.ToString().Split(Environment.NewLine.ToCharArray());
                lines = lines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    foreach (String line in lines)
                    {
                        string[] columns = line.Split(',');
                        if (columns[4] != null)
                        {
                            string detailsLine = columns[4].Replace("\"", "");
                            string custName = columns[51].Replace("\"", "");
                            string custZip = columns[57].Replace("\"", "").Replace("=", "");
                            Order order =
                               _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusInProgress &&
                                                                detailsLine.Contains(a.CustName.Replace(",", ""), StringComparison.InvariantCultureIgnoreCase) &&
                                                                detailsLine.Contains(a.CustZipCode.Substring(0, 5).Replace(",", ""), StringComparison.InvariantCultureIgnoreCase) &&
                                                                columns[18].Contains(a.ProductName, StringComparison.InvariantCultureIgnoreCase) &&
                                                                columns[18].Contains(a.Quantity.ToString(), StringComparison.InvariantCultureIgnoreCase) &&
                                                                a.TrackingNumber == null)
                                                         .FirstOrDefault();
                            if (order != null)
                            {
                                order.TrackingNumber = columns[7].Replace("\"", "").Replace("=", "");
                                order.Carrier = columns[9].Replace("\"", "");
                                order.OrderStatus = SD.OrderStatusDone;
                                _db.Orders.Update(order);
                            }
                        }
                    }
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                success = 1;
                excep = "Tracking Updated Succesfully!";
            }
            catch
            {
                success = 0;
                excep = "There was an Error, some orders were not updated!";
            }
            return Json(new { excep, success });
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
