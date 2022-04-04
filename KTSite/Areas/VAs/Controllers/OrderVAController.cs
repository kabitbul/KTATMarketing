 using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using KTSite.DataAccess.Data;
using System.IO;
using ExcelDataReader;
using System.Drawing;

namespace KTSite.Areas.VAs.Controllers
{
    [Area("VAs")]
    [Authorize(Roles = SD.Role_VAs)]
    public class OrderVAController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public OrderVAController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.ComplaintsPending = _unitOfWork.Complaints.GetAll().Where(a => !a.WarehouseResponsibility && !a.Solved).Count();
            ViewBag.TaskFromAdmin = _unitOfWork.adminVATask.GetAll().Where(q => !q.TaskCompleted).
                    Join(_unitOfWork.ApplicationUser.GetAll().
                Where(a => a.Role == SD.Role_Admin),
                                             adminVATask => adminVATask.UserNameId,
                                             applicationUser => applicationUser.Id,
                                             (adminVATask, applicationUser) => new
                                             {
                                                 adminVATask
                                             }).Select(a => a.adminVATask).Count();
            //ViewBag.ReturnLabelRefund = _unitOfWork.ReturnLabel.GetAll().Where(a => a.ReturnDelivered && !returnIsRefunded(a.OrderId)).Count();
            dynamic myModel = new System.Dynamic.ExpandoObject();
            ViewBag.errSaveInProgress = false;
            return View(myModel);
        }
        public IActionResult AddOrdersShopsFB()
        {
            string uNameId = "";
            string uName = "";
            ApplicationUser appUser = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).FirstOrDefault());
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            ExcelUploadsForShopsVM excelUploadsForShopsVM = new ExcelUploadsForShopsVM()
            {
                excelUploadsForShops = new ExcelUploadsForShops(),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.IsAdminStore && !_unitOfWork.excelUploadsForShops.GetAll().
                                                                        Any(a =>  a.StoreId == q.Id && !a.TrackingUpdated))
                .Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                }),
                StoresListTR = _unitOfWork.UserStoreName.GetAll().Where(q => q.IsAdminStore && _unitOfWork.excelUploadsForShops.GetAll().
                                                                       Any(a => a.StoreId == q.Id && !a.TrackingUpdated))
                .Select(i => new SelectListItem
                {
                    Text = i.StoreName + "-fromOrdId-" + i.FromOrdId + "-toOrdId-" + i.ToOrdId,
                    Value = i.Id.ToString()
                })
            };
            ViewBag.UNameId = uNameId;
            ViewBag.failed = "";
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.InsufficientFunds = false;
            ViewBag.ShowErrInLabel = false;
            return View(excelUploadsForShopsVM);
        }
        //public bool returnIsRefunded(long OrderId)
        //{
        //    return _unitOfWork.Refund.GetAll().Any(a => a.OrderId == OrderId);
        //}
        //allow return if not returned yet or returned part of total quantity
        public bool allowRetrun(string orderId)
        {
            ReturnLabel returnLabel = _unitOfWork.ReturnLabel.GetAll().Where(a => a.OrderId == Convert.ToInt64(orderId)).FirstOrDefault();
            if (returnLabel == null)
            {
                return true;
            }
            return false;
        }
        public bool allowComplaint(string orderId)
        {
            Complaints complaints = _unitOfWork.Complaints.GetAll().Where(a => a.OrderId == Convert.ToInt64(orderId)).FirstOrDefault();
            if (complaints == null)
            {
                return true;
            }
            return false;
        }
        public IActionResult AddOrdersManually()
        {
            string uNameId = "";
            string uName = "";
            uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            OrderVM orderVM = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers &&(a.AdminApproval == null || a.AdminApproval == SD.MerchProductStatusApproved) &&
                                                                         a.MerchId != SD.Kfir_Merch).
                OrderBy(a => a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.IsAdminStore).Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                }),
                StatesList = SD.States
            };
            
            ViewBag.UNameId = uNameId;
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            return View(orderVM);
        }
        public IActionResult UpdateOrder(int id)
        {
            string uNameId = "";
            string uName = "";
            uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            OrderVM orderVM = new OrderVM()
            {
                Orders = _unitOfWork.Order.GetAll().Where(a => a.Id == id).FirstOrDefault(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && (a.AdminApproval == null || a.AdminApproval == SD.MerchProductStatusApproved)&&
                                                                 a.MerchId != SD.Kfir_Merch).
                OrderBy(a => a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.IsAdminStore == true).Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                }),
                StatesList = SD.States,
                StatusList = SD.StatusAcceptOrCancel
            };
            ViewBag.UNameId = uNameId;
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            return View(orderVM);
        }
        public IActionResult AddOrdersExtension()
        {
            string uNameId = "";
            string uName = "";
            uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            OrderVM orderVM = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && (a.AdminApproval == null || a.AdminApproval == SD.MerchProductStatusApproved) &&
                                                                 a.MerchId != SD.Kfir_Merch).
                OrderBy(a => a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.IsAdminStore).Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                }),
                StatesList = SD.States
            };
            ViewBag.UNameId = uNameId;
            ViewBag.failed = "";
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.ShowErrInLabel = false;
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
                //order.ProductName = returnProductName(order.ProductId);
                order.StringDate = order.UsDate.Day + "/" + order.UsDate.Month + "/" + order.UsDate.Year;
                //order.UserName =  returnUserOrAdminName(order.StoreNameId);
                //order.StoreName = returnStoreName(order.StoreNameId);
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
                                            x.UserNameToShow.ToLower().Contains(searchValue.ToLower()) ||
                                            x.StoreName.ToLower().Contains(searchValue.ToLower()) ||
                                            x.Quantity.ToString().Contains(searchValue.ToLower()) ||
                                            x.Cost.ToString().Contains(searchValue.ToLower()) ||
                                            (!string.IsNullOrEmpty(x.TrackingNumber) && x.TrackingNumber.ToString().Contains(searchValue.ToLower()))||
                                            (!string.IsNullOrEmpty(x.MerchType) && x.MerchType.ToString().Contains(searchValue.ToLower()))
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
                else if (sortColumnName.ToLower() == "usernametoshow")
                {
                    orderList = orderList.OrderByDescending(x => x.UserNameToShow).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "storename")
                {
                    orderList = orderList.OrderByDescending(x => x.StoreName).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "quantity")
                {
                    orderList = orderList.OrderByDescending(x => x.Quantity).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "cost")
                {
                    orderList = orderList.OrderByDescending(x => x.Cost).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "fulladdress")
                {
                    orderList = orderList.OrderByDescending(x => x.FullAddress).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "trackingnumber")
                {
                    orderList = orderList.OrderByDescending(x => x.TrackingNumber).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "merchtype")
                {
                    orderList = orderList.OrderByDescending(x => x.MerchType).ToList<Order>();
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
                else if (sortColumnName.ToLower() == "usernametoshow")
                {
                    orderList = orderList.OrderBy(x => x.UserNameToShow).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "storename")
                {
                    orderList = orderList.OrderBy(x => x.StoreName).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "quantity")
                {
                    orderList = orderList.OrderBy(x => x.Quantity).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "cost")
                {
                    orderList = orderList.OrderBy(x => x.Cost).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "fulladdress")
                {
                    orderList = orderList.OrderBy(x => x.FullAddress).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "trackingnumber")
                {
                    orderList = orderList.OrderBy(x => x.TrackingNumber).ToList<Order>();
                }
                else if (sortColumnName.ToLower() == "merchtype")
                {
                    orderList = orderList.OrderBy(x => x.MerchType).ToList<Order>();
                }
            }
            //orderList = orderList.OrderBy(x => x.Id //sortColumnName + " " + sortDirection).ToList<Order>();
            orderList = orderList.Skip(start).Take(length).ToList<Order>();
            foreach (Order order in orderList)
            {
                order.AllowComplaint = allowComplaint(order.Id.ToString());
                order.AllowReturn = allowRetrun(order.Id.ToString());
                order.isChecked = false;
            }
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
        public IActionResult AddOrdersManually(OrderVM orderVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.uNameId = uNameId;
            if (ModelState.IsValid)
            {
                Product pr = _unitOfWork.Product.Get(orderVM.Orders.ProductId);

                if (pr.MerchType == SD.Role_KTMerch)
                {
                    orderVM.Orders.Cost = Convert.ToDouble(String.Format("{0:0.00}", (pr.SellersCost * orderVM.Orders.Quantity)));
                    orderVM.Orders.MerchType = SD.Role_KTMerch;
                    orderVM.Orders.MerchId = pr.MerchId;
                }
                else if (pr.MerchType == SD.Role_ExMerch)
                {
                    orderVM.Orders.Cost = Convert.ToDouble(String.Format("{0:0.00}", (pr.SellersCost * orderVM.Orders.Quantity)));
                    orderVM.Orders.MerchType = SD.Role_ExMerch;
                    orderVM.Orders.MerchId = pr.MerchId;
                }
                else
                {
                    orderVM.Orders.Cost = Convert.ToDouble(String.Format("{0:0.00}", (pr.Cost * orderVM.Orders.Quantity)));
                }
                orderVM.Orders.Cost = returnCost(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                if (isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now && orderVM.Orders.MerchId != SD.Kfir_Merch)
                {
                    using (var dbContextTransaction = _db.Database.BeginTransaction())
                    { 
                    bool fail = false;
                    try
                    {
                        orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                        orderVM.Orders.UserNameToShow = "Admin";
                        orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                        orderVM.Orders.CustName = orderVM.Orders.CustName.Trim();
                        _unitOfWork.Order.Add(orderVM.Orders);
                        updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                        updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                    }
                    catch
                    {
                        fail = true;
                    }
                    if (!fail)
                    {
                            //_unitOfWork.Save();
                            _db.SaveChanges();
                            dbContextTransaction.Commit();
                            ViewBag.success = true;
                    }

                    ViewBag.failed = false;
                }
                }
                else
                {
                    ViewBag.failed = true;
                }
            }
            OrderVM orderVM2 = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && (a.AdminApproval == null || a.AdminApproval == SD.MerchProductStatusApproved) &&
                                                                 a.MerchId != SD.Kfir_Merch).
                OrderBy(a => a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.IsAdminStore).Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                }),
                StatesList = SD.States
            };
            return View(orderVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrder(OrderVM orderVM)
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.uNameId = uNameId;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                orderVM.Orders.Cost = returnCost(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                if (isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now && orderVM.Orders.MerchId != SD.Kfir_Merch)
                {
                    Order ord = _unitOfWork.Order.Get(orderVM.Orders.Id);
                    int oldQuantity = ord.Quantity;
                    int oldProductId = ord.ProductId;
                    string oldStatus = ord.OrderStatus;
                    bool fail = false;
                    using (var dbContextTransaction = _db.Database.BeginTransaction())
                    {
                        try
                        {
                            //changed to cancel
                            if (orderVM.Orders.OrderStatus != oldStatus && orderVM.Orders.OrderStatus == SD.OrderStatusCancelled)
                            {
                                updateInventory(oldProductId, oldQuantity * (-1));
                                updateWarehouseBalance(oldQuantity * (-1), oldProductId);
                                // if it's a cancellation - we dont want any change but the cancellation it self
                                orderVM.Orders = _unitOfWork.Order.Get(orderVM.Orders.Id);
                                orderVM.Orders.OrderStatus = SD.OrderStatusCancelled;
                            }
                            //change from cancel
                            else if (orderVM.Orders.OrderStatus != oldStatus && orderVM.Orders.OrderStatus != SD.OrderStatusCancelled && oldStatus == SD.OrderStatusCancelled)
                            {
                                updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                                updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                            }
                            //status didnt change
                            else if (orderVM.Orders.OrderStatus == oldStatus && orderVM.Orders.OrderStatus != SD.OrderStatusCancelled)
                            {
                                if (oldQuantity != orderVM.Orders.Quantity)
                                {
                                    updateInventory(orderVM.Orders.ProductId, (orderVM.Orders.Quantity - oldQuantity));
                                    updateWarehouseBalance(orderVM.Orders.Quantity - oldQuantity, orderVM.Orders.ProductId);
                                }
                            }
                            orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                            orderVM.Orders.UserNameToShow = "Admin";
                            orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                            orderVM.Orders.CustName = orderVM.Orders.CustName.Trim();
                            _unitOfWork.Order.update(orderVM.Orders);
                        }
                        catch
                        {
                            fail = true;
                        }
                        if (!fail)
                        {
                            //_unitOfWork.Save();
                            _db.SaveChanges();
                            dbContextTransaction.Commit();
                            ViewBag.success = true;
                        }

                        ViewBag.failed = false;
                    }
                }
                else
                {
                    ViewBag.failed = true;
                }
            }
            OrderVM orderVM2 = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && (a.AdminApproval == null || a.AdminApproval == SD.MerchProductStatusApproved) &&
                                                                 a.MerchId != SD.Kfir_Merch).
                OrderBy(a => a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.UserNameId == uNameId).Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                }),
                StatesList = SD.States,
                StatusList = SD.StatusAcceptOrCancel
            };
            return View(orderVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrdersExtension(OrderVM orderVM)
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.uNameId = uNameId;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            ViewBag.ShowErrInLabel = false;
            int processedLines = 0;
            if (ModelState.IsValid)
            {
                string allOrders = orderVM.AllOrder;
                if(allOrders != null && allOrders.Length > 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (allOrders != null && allOrders.Length > 2)
                        {
                            if (allOrders[(allOrders.Length - 1)].Equals('\r') || allOrders[(allOrders.Length - 1)].Equals('\n') ||
                            allOrders[(allOrders.Length - 1)].Equals('\"'))
                            {
                                allOrders = allOrders.Remove(allOrders.Length - 1);
                            }
                        }
                    }
                    
                }
                else
                {
                    ViewBag.ShowMsg = false;
                    return View(orderVM);
                }
                var ordersList = allOrders.Split(new string[] { "\"\r\n" },StringSplitOptions.None);
                string failedLines = "";
                int lineNum = 0;
                foreach (var order in ordersList)
                {
                    using (var dbContextTransaction = _db.Database.BeginTransaction())
                    {
                        initializeVM(orderVM);
                    lineNum++;
                    try
                    {
                        var orderDetails = order.Split(new string[] { "\t" }, StringSplitOptions.None);
                        addAddressDetailsToVM(orderDetails[4], orderVM);
                        orderVM.Orders.ProductId = getProductIdByName(orderDetails[0]);
                        orderVM.Orders.UserNameId = returnUserNameId();
                        orderVM.Orders.StoreNameId = getStoreNameId(orderDetails[1]);
                        orderVM.Orders.Quantity = Int32.Parse(orderDetails[3]);
                        if (orderVM.Orders.ProductId > 0)
                        {
                            Product pr = _unitOfWork.Product.Get(orderVM.Orders.ProductId);
                            if (pr.MerchType == SD.Role_KTMerch)
                            {
                                orderVM.Orders.MerchType = SD.Role_KTMerch;
                                orderVM.Orders.MerchId = pr.MerchId;
                            }
                            else if (pr.MerchType == SD.Role_ExMerch)
                            {
                                orderVM.Orders.MerchType = SD.Role_ExMerch;
                                orderVM.Orders.MerchId = pr.MerchId;
                            }

                            orderVM.Orders.Cost = returnCost(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                        }
                        string validDate;
                        if (orderDetails[2].Length < 10)
                        {
                            var splitDate = orderDetails[2].Split(new string[] { "/" }, StringSplitOptions.None);
                            if (splitDate[0].Length == 1)
                            {
                                validDate = "0" + splitDate[0] + "/";
                            }
                            else
                            {
                                validDate = splitDate[0] + "/";
                            }
                            if (splitDate[1].Length == 1)
                            {
                                validDate = validDate + "0" + splitDate[1] + "/";
                            }
                            else
                            {
                                validDate = validDate + splitDate[1] + "/";
                            }
                            validDate = validDate + splitDate[2];
                        }
                        else
                        {
                            validDate = orderDetails[2];
                        }
                        orderVM.Orders.UsDate = DateTime.ParseExact(validDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        //remove diacritics and comma
                        orderVM.Orders.CustName = RemoveDiacritics(orderVM.Orders.CustName).Replace(",", "").Trim();
                        orderVM.Orders.CustStreet1 = RemoveDiacritics(orderVM.Orders.CustStreet1).Replace(",", "");
                        if (orderVM.Orders.CustStreet2.Length > 0)
                        {
                            orderVM.Orders.CustStreet2 = RemoveDiacritics(orderVM.Orders.CustStreet2).Replace(",", "");
                        }
                        if (isStoreAuthenticated(orderVM) && orderVM.Orders.ProductId > 0 &&
                            orderVM.Orders.UsDate <= DateTime.Now && Enumerable.Range(1, 100).Contains(orderVM.Orders.Quantity) &&
                            orderVM.Orders.CustName.Length > 0 && orderVM.Orders.CustStreet1.Length > 0 &&
                            Enumerable.Range(5, 10).Contains(orderVM.Orders.CustZipCode.Length) &&
                            orderVM.Orders.CustCity.Length > 1 && orderVM.Orders.CustState.Length == 2 &&
                            orderVM.Orders.MerchId != SD.Kfir_Merch)
                        {
                            orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                            orderVM.Orders.UserNameToShow = "Admin";
                            orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                            _unitOfWork.Order.Add(orderVM.Orders);
                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                                //_unitOfWork.Save();
                                _db.SaveChanges();
                                dbContextTransaction.Commit();
                                processedLines++;
                        }
                        else
                        {
                            ViewBag.success = false;
                            if (failedLines.Length == 0)
                            {
                                failedLines = orderVM.Orders.CustName;
                            }
                            else
                            {
                                failedLines = failedLines + "/n" + orderVM.Orders.CustName;
                            }
                        }
                    }
                    catch
                    {
                        ViewBag.success = false;
                        if (failedLines.Length == 0)
                        {
                            failedLines = orderVM.Orders.CustName;
                        }
                        else
                        {
                            failedLines = failedLines + "/n" + orderVM.Orders.CustName;
                        }
                    }
                }

                }
                if(processedLines > 0 )
                {
                    ViewBag.success = true;
                }
                if (failedLines.Length == 0)
                {
                    ViewBag.failed = "";
                }
                else
                {
                    ViewBag.success = false;
                    if (processedLines == 0)
                    {
                        ViewBag.failed = "Pay Attention: An error occured! No Orders were processed!";
                    }
                    else if (processedLines == 1)
                    {
                        ViewBag.ShowErrInLabel = true;
                        ViewBag.failed = "Only One Order was Processed Successfully!/n" +
                        "/nfailed Orders/n/n: " + failedLines;
                    }
                    else
                    {
                        ViewBag.ShowErrInLabel = true;
                        ViewBag.failed = "An error occured Only " + processedLines + " Orders were Processed Successfully!/n" +
                     "/nfailed Orders:/n/n" + failedLines;
                    }
                }
                ViewBag.processedLines = processedLines;
                return View(orderVM);

                //}
                //return RedirectToAction(nameof(Index));
            }
            ViewBag.processedLines = processedLines;
            return View(orderVM.Orders);
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
            string prd = productName.Trim();
            return _unitOfWork.Product.GetAll().Where(a => a.ProductName.Equals(prd, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id).FirstOrDefault();
        }
        public int getStoreNameId(string storeName)
        {
            return _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore
            && a.StoreName.Equals(storeName, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id).FirstOrDefault();
        }
        public void addAddressDetailsToVM(string orderDetails, OrderVM orderVM)
        {
            var addressDetails = orderDetails.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            //always appear
            orderVM.Orders.CustName = addressDetails[0];
            orderVM.Orders.CustName = orderVM.Orders.CustName.Substring(1);
            orderVM.Orders.CustStreet1 = addressDetails[1];
            if(addressDetails.Length == 4)
            {
                cityStateZipToVM(addressDetails[2], orderVM);
            }
            else if(addressDetails.Length == 5)
            {
                //if last rec is unitedstates then 2 street and no phone
                if (addressDetails[addressDetails.Length - 1] == "United States")
                {
                    orderVM.Orders.CustStreet2 = addressDetails[2];
                    cityStateZipToVM(addressDetails[3], orderVM);
                    orderVM.Orders.CustPhone = "999-999-9999";///phone
                }
                //else 1 street and a phone
                else
                {
                    cityStateZipToVM(addressDetails[2], orderVM);
                    orderVM.Orders.CustPhone = addressDetails[4];
                }
            }
            else if (addressDetails.Length == 6)
            {
                orderVM.Orders.CustStreet2 = addressDetails[2];
                cityStateZipToVM(addressDetails[3], orderVM);
                orderVM.Orders.CustPhone = addressDetails[5];
            }
        }
        public void cityStateZipToVM(string line, OrderVM orderVM)
        {
            var cityStateZip = line.Trim().Split(' ');
            cityStateZip = cityStateZip.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            orderVM.Orders.CustZipCode = cityStateZip[cityStateZip.Length - 1];
            orderVM.Orders.CustState = cityStateZip[cityStateZip.Length - 2];
            if (orderVM.Orders.CustState.Length > 2)// if not an abbr 
            {
                foreach (var state in SD.States)
                {
                    if (state.Text.ToLower().ToString().Contains(orderVM.Orders.CustState))
                    {
                        orderVM.Orders.CustState = state.Value;
                    }
                }
            }
            orderVM.Orders.CustCity = cityStateZip[0];
            for (int i = 1; i < (cityStateZip.Length - 2); i++)
            {
                orderVM.Orders.CustCity = orderVM.Orders.CustCity + " " + cityStateZip[i];
            }
        }
        public string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public void initializeVM(OrderVM orderVM)
        {
            orderVM.Orders.Id = 0;
            orderVM.Orders.ProductId = 0;
            orderVM.Orders.CustStreet2 = "";
            orderVM.Orders.CustStreet1 = "";
            orderVM.Orders.CustCity = "";
            orderVM.Orders.CustState = "";
            orderVM.Orders.CustZipCode = "";
            orderVM.Orders.CustPhone = "";
            orderVM.Orders.IsAdmin = true;
            orderVM.Orders.OrderStatus = SD.OrderStatusAccepted;
            orderVM.Orders.MerchType = "";
            orderVM.Orders.MerchId = "";
        }
        public void updateInventory(int productId, int quantity)
        {
            Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            //if(product.MerchType != SD.Role_ExMerch)
            product.InventoryCount = product.InventoryCount - quantity;
        }
        public void updateWarehouseBalance(int quantity, int productId)
        {
            Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            PaymentBalance warehousePaymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).FirstOrDefault();
            if (product.MerchType == SD.Role_KTMerch)
            {
                //pay warehouse
                //paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost));
                warehousePaymentBalance.Balance = warehousePaymentBalance.Balance - (quantity * (product.ShippingCharge));
                //pay merch --product cost minus shipping minus feeprecent SD.FeesOfKTMerch
                PaymentBalanceMerch KTMerchPaymentBalance = _unitOfWork.PaymentBalanceMerch.GetAll().Where(a => a.UserNameId == product.MerchId).FirstOrDefault();
                double totalProfit = 0.0;
                totalProfit = (product.SellersCost * quantity * (1 - SD.FeesOfKTMerch)) - ((product.ShippingCharge ) * quantity);
                totalProfit = Math.Round(totalProfit, 2);
                KTMerchPaymentBalance.Balance = KTMerchPaymentBalance.Balance + totalProfit;

            }
            else if (product.MerchType == SD.Role_ExMerch)
            {
                //payMerch - add 
                PaymentBalanceMerch EXMerchPaymentBalance = _unitOfWork.PaymentBalanceMerch.GetAll().Where(a => a.UserNameId == product.MerchId).FirstOrDefault();
                EXMerchPaymentBalance.Balance = EXMerchPaymentBalance.Balance + (product.SellersCost * quantity * (1 - SD.FeesOfEXMerch));
            }
            else if (product.OwnByWarehouse)
            {
                //paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost_warehouse_items+product.Cost));
                warehousePaymentBalance.Balance = warehousePaymentBalance.Balance - (quantity * (product.ShippingCharge + product.Cost));
            }
            else
            {
                //paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost));
                warehousePaymentBalance.Balance = warehousePaymentBalance.Balance - (quantity * (product.ShippingCharge));
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll();
            return Json(new { data = allObj });
        }
        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var objFromDb = _unitOfWork.Product.Get(id);
        //    if(objFromDb == null)
        //    {
        //        return Json(new { success = false, message = "Error While Deleting" });
        //    }
        //    _unitOfWork.Product.Remove(objFromDb);
        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "Delete Successfull" });
        //}

        #endregion
        [HttpPost]
        public IActionResult TrackUpdate(int[] Ids)
        {
            foreach (int Id in Ids)
            {
                Order order = _unitOfWork.Order.Get(Id);
                if (order.TrackingUpdated)
                    order.TrackingUpdated = false;
                else
                    order.TrackingUpdated = true;
                _unitOfWork.Save();
            }
            return View();
        }
        [HttpPost]
        //read the orders coming from facebook excel
        public JsonResult SubmitForShops(fileAndStoreIdVM fVM)//IFormFile CSVFile, int storeId)
        {
            int success = 0;
            string excep = "";
            var result = new StringBuilder();
            bool existFail = false;
            int proccessedOrders = 0;
            int countRec = 0;
            string UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    fVM.CSVFile.CopyTo(stream);
                    stream.Position = 0;
                    long fromOrderId = 0;
                    bool insufficientFunds = false;
                    if(!fVM.CSVFile.FileName.ToUpper().EndsWith("XLSX"))
                    {
                        success = 0;
                         excep = "File must be of type xlsx (Excel file)";
                        return Json(new { excep, success });
                    }

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        using (var dbContextTransaction = _db.Database.BeginTransaction())
                        {
                            while (reader.Read()) //Each row of the file
                            {
                                try
                                {
                                    string skuNamefromExcel = "";
                                    if (reader.GetValue(3) != null)
                                    {
                                        skuNamefromExcel = reader.GetValue(3).ToString();
                                    }
                                    int prodId = getProductIdByName(skuNamefromExcel);
                                    if (prodId > 0)
                                    {
                                        Order ord = new Order();
                                        ord.CustName = reader.GetValue(5).ToString();
                                        ord.CustZipCode = reader.GetValue(10).ToString();
                                        ord.Quantity = Int32.Parse(reader.GetValue(4).ToString());
                                        ord.CustStreet1 = reader.GetValue(6).ToString();
                                        if (ord.CustStreet1 != null)
                                        {
                                            ord.CustStreet1 = ord.CustStreet1.Replace(",", "");
                                        }

                                        if (reader.GetValue(7) != null && reader.GetValue(7) != "")
                                        {
                                            ord.CustStreet2 = reader.GetValue(7).ToString().Replace(",", "");
                                        }
                                        ord.CustCity = reader.GetValue(8).ToString();
                                        ord.CustState = reader.GetValue(9).ToString();
                                        ord.ProductId = prodId;
                                        ord.ProductName = skuNamefromExcel;
                                        ord.StoreName = _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == fVM.storeId).Select(a => a.StoreName).FirstOrDefault();
                                        ord.UserNameToShow = "Admin";
                                        ord.Cost = returnCost(prodId, ord.Quantity);
                                        //_unitOfWork.Product.GetAll().Where(q => q.Id == prodId).Select(a => a.SellersCost).FirstOrDefault();
                                        ord.StoreNameId = fVM.storeId;
                                        ord.OrderStatus = SD.OrderStatusAccepted;
                                        ord.UserNameId = returnUserNameId();
                                        ord.UsDate = DateTime.Now.Date;

                                        OrderVM orderVM = new OrderVM();
                                        orderVM.Orders = ord;
                                        if (isStoreAuthenticated(orderVM) && orderVM.Orders.ProductId > 0 &&
                                            orderVM.Orders.UsDate <= DateTime.Now && Enumerable.Range(1, 100).Contains(orderVM.Orders.Quantity) &&
                                            orderVM.Orders.CustName.Length > 0 && orderVM.Orders.CustStreet1.Length > 0 &&
                                            Enumerable.Range(5, 10).Contains(orderVM.Orders.CustZipCode.Length) &&
                                            orderVM.Orders.CustCity.Length > 1 && orderVM.Orders.CustState.Length == 2 &&
                                            orderVM.Orders.MerchId != SD.Kfir_Merch)
                                        {
                                            orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                                            orderVM.Orders.UserNameToShow = "Admin";
                                            orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);

                                            _db.Orders.Add(ord);
                                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                                            //updateSellerBalance(orderVM.Orders.Cost);
                                            _unitOfWork.Save();
                                        }
                                        ///////////////////////////////////////////
                                        //_unitOfWork.Order.Add(ord);

                                        proccessedOrders++;
                                        //_unitOfWork.Save();

                                        if (countRec == 0)//first time, create rec
                                        {

                                        }
                                        countRec++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    existFail = true;
                                }
                            }
                            ExcelUploadsForShops exUp = new ExcelUploadsForShops();
                            long toOrd = _db.Orders.Max(o => o.Id);
                            exUp.UserId = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id).FirstOrDefault();
                            exUp.FromOrderId = toOrd - (proccessedOrders - 1);
                            exUp.TrackingUpdated = false;
                            exUp.CreatedDate = DateTime.Now; exUp.StoreId = fVM.storeId;
                            exUp.ToOrderId = toOrd;
                            _unitOfWork.excelUploadsForShops.Add(exUp);
                            UserStoreName usStore = _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == fVM.storeId).FirstOrDefault();
                            usStore.FromOrdId = toOrd - (proccessedOrders - 1);
                            usStore.ToOrdId = toOrd;
                            _unitOfWork.UserStoreName.update(usStore);
                            _db.SaveChanges();
                            dbContextTransaction.Commit();
                            // }
                        }

                        if (!existFail)
                        {
                            success = 1;
                            excep = proccessedOrders + " Orders Added successfully!";
                        }
                        else
                        {
                            success = 0;
                            excep = "There was an Error, some orders were not proccessed: ";
                        }
                    }
                }
                return Json(new { excep, success });
            }
            catch (Exception ex)
            {
                success = 0;
                excep = "There was an Error, some orders were not proccessed: ";
                return Json(new { excep, success });

            }
        }
        //create excel with tracking numbers to upload to shops
        [HttpPost]
        public ActionResult Export(ExcelUploadsForShopsVM excelUploadsForShopsVM)
        {
            ExcelUploadsForShops ex = _unitOfWork.excelUploadsForShops.GetAll().Where(a => a.StoreId == excelUploadsForShopsVM.excelUploadsForShops.StoreId && !a.TrackingUpdated)
                .FirstOrDefault();
            string storeName = _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == ex.StoreId).Select(a => a.StoreName).FirstOrDefault();
            string fileName = storeName + "_" + DateTime.Now.DayOfWeek + "_HH" + DateTime.Now.Hour + "_MI" + DateTime.Now.Minute + ".csv";
            IEnumerable<Order> orderList = _unitOfWork.Order.GetAll().Where(a => a.Id >= ex.FromOrderId && a.Id <= ex.ToOrderId).
                                           OrderBy(a => a.Id);

            StringBuilder sb = new StringBuilder();
            //Header
            int lineCounter = 0;
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (Order order in orderList)
                    {
                        sb.Append(order.Quantity.ToString());
                        sb.Append(',');
                        sb.Append("USPS");
                        sb.Append(',');
                        sb.Append("=\"");
                        sb.Append(order.TrackingNumber);
                        sb.Append("\"");
                        //_unitOfWork.Order.Get(order.Id).OrderStatus = SD.OrderStatusDone;
                        _unitOfWork.Order.Get(order.Id).TrackingUpdated = true;
                        _db.Orders.Update(_unitOfWork.Order.Get(order.Id));
                        sb.Append("\r\n");
                        lineCounter++;
                    }
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                    //update when file downloaded
                    ex.TrackingUpdated = true;
                    _unitOfWork.Save();
                }
                catch
                {
                    ViewBag.errSaveInProgress = true;
                }
            }
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", fileName);
        }
    }
}
