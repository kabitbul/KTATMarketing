using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Globalization;
using System.Text;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [Authorize(Roles = SD.Role_Users)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            dynamic myModel = new System.Dynamic.ExpandoObject();
            OrderVM orderVM = new OrderVM();
            myModel.OrderVM = orderVM;//_unitOfWork.Order.GetAll().Where(q=>q.UserNameId == UNameId).Where(q => q.IsAdmin == false);
            ViewBag.errSaveInProgress = false;
            ViewBag.uNameId = UNameId;
            return View(myModel);
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
                ProductList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers && !a.OOSForSellers).
                OrderBy(a=>a.ProductName).Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.UserNameId == uNameId).Select(i => new SelectListItem
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
            ViewBag.InsufficientFunds = false;
            ViewBag.failed = false;
            orderVM.Orders.UsDate = DateTime.Now;
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
                Orders = _unitOfWork.Order.GetAll().Where(a=> a.Id == id).FirstOrDefault(),
                ProductList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers && !a.OOSForSellers).
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
            ViewBag.UNameId = uNameId;
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            ViewBag.InsufficientFunds = false;
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
                ProductList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers && !a.OOSForSellers).
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
                StatesList = SD.States
            };
            ViewBag.UNameId = uNameId;
            ViewBag.failed = "";
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.InsufficientFunds = false;
            ViewBag.ShowErrInLabel = false;
            return View(orderVM);
        }
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
            if (complaints == null )
            {
                return true;
            }
            return false;
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
            double productCost = (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.SellersCost)).FirstOrDefault();
            return Convert.ToDouble(String.Format("{0:0.00}", (productCost * quantity))); 
        }
        public PaymentBalance userBalance(string userNameId)
        {
            return
            _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == userNameId).FirstOrDefault();
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
            string userNameId = returnUserNameId();
            orderList = _unitOfWork.Order.GetAll().Where(a=>a.UserNameId == userNameId).ToList();
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
                                            x.Cost.ToString().Contains(searchValue.ToLower()) ||
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
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.uNameId = uNameId;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            PaymentBalance paymentBalance = userBalance(uNameId);
            OrderVM orderVM2 = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers && !a.OOSForSellers).
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
                StatesList = SD.States
            };
            if (ModelState.IsValid)
            {
                orderVM.Orders.Cost = returnCost(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                if (!paymentBalance.AllowNegativeBalance && paymentBalance.Balance < orderVM.Orders.Cost)
                {
                    ViewBag.InsufficientFunds = true;
                    ViewBag.failed = false;
                    return View(orderVM2);
                }
                ViewBag.InsufficientFunds = false;
                if (isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now)
                {
                    orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                    orderVM.Orders.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                    orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                    orderVM.Orders.CustName = orderVM.Orders.CustName.Trim();
                    _unitOfWork.Order.Add(orderVM.Orders);
                    updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                    updateSellerBalance(orderVM.Orders.Cost);
                    updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                    _unitOfWork.Save();
                    ViewBag.success = true;
                    ViewBag.failed = false;
                }
                else
                {
                    ViewBag.failed = true;
                }
            }
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
            OrderVM orderVM2 = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().Where(a=>a.AvailableForSellers && !a.OOSForSellers).
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
            if (ModelState.IsValid)
            {
                orderVM.Orders.Cost = returnCost(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                if (isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now)
                {
                    Order ord = _unitOfWork.Order.Get(orderVM.Orders.Id);
                    int oldQuantity = ord.Quantity;
                    int oldProductId = ord.ProductId;
                    double oldCost = ord.Cost;
                    string oldStatus = ord.OrderStatus;
                    PaymentBalance paymentBalance = userBalance(uNameId);
                    if (!paymentBalance.AllowNegativeBalance && paymentBalance.Balance < (orderVM.Orders.Cost - oldCost))
                    {
                        ViewBag.InsufficientFunds = true;
                        ViewBag.failed = false;
                        return View(orderVM2);
                    }
                    bool fail = false;
                    try
                    {
                        //changed to cancel
                        if (orderVM.Orders.OrderStatus != oldStatus && orderVM.Orders.OrderStatus == SD.OrderStatusCancelled)
                        {
                            updateInventory(oldProductId, oldQuantity * (-1));
                            updateWarehouseBalance(oldQuantity * (-1), oldProductId);
                            updateSellerBalance(oldCost*(-1));
                            // if it's a cancellation - we dont want any change but the cancellation it self
                            orderVM.Orders = _unitOfWork.Order.GetAll().Where(a => a.Id == orderVM.Orders.Id).FirstOrDefault();
                            orderVM.Orders.OrderStatus = SD.OrderStatusCancelled;
                        }
                        //change from cancel
                        else if (orderVM.Orders.OrderStatus != oldStatus && orderVM.Orders.OrderStatus != SD.OrderStatusCancelled && oldStatus == SD.OrderStatusCancelled)
                        {
                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                            updateSellerBalance(orderVM.Orders.Cost);
                        }
                        //status didnt change
                        else if (orderVM.Orders.OrderStatus == oldStatus && orderVM.Orders.OrderStatus != SD.OrderStatusCancelled)
                        {
                            if (oldQuantity != orderVM.Orders.Quantity)
                            {
                                updateInventory(orderVM.Orders.ProductId, (orderVM.Orders.Quantity - oldQuantity));
                                updateWarehouseBalance((orderVM.Orders.Quantity - oldQuantity), orderVM.Orders.ProductId);
                                updateSellerBalance(orderVM.Orders.Cost - oldCost);
                            }
                        }
                        orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                        orderVM.Orders.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
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
                        _unitOfWork.Save();
                        ViewBag.success = true;
                    }
                    ViewBag.failed = false;
                }
                else
                {
                    ViewBag.failed = true;
                }
            }
            ViewBag.InsufficientFunds = false;
            return View(orderVM2);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void CancelOrder(int? orderId)
        {
            if(orderId != null)
            {
                Order order = _unitOfWork.Order.Get(Convert.ToInt64(orderId));
                order.OrderStatus = SD.OrderStatusCancelled;
                order.ProductName = returnProductName(order.ProductId);
                order.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                order.StoreName = returnStoreName(order.StoreNameId);
                _unitOfWork.Order.update(order);
                updateInventory(order.ProductId, (order.Quantity*(-1)));
                updateWarehouseBalance((order.Quantity * (-1)), order.ProductId);
                updateSellerBalance((order.Cost * (-1)));
                _unitOfWork.Save();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrdersExtension(OrderVM orderVM)
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.uNameId = uNameId;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            ViewBag.failed = "";
            ViewBag.InsufficientFunds = false;
            ViewBag.ShowErrInLabel = false;
            int processedLines = 0;
            ViewBag.ProcessedLines = processedLines;
            bool InsufficientFunds = false;
            
            if (ModelState.IsValid)
            {
                string allOrders = orderVM.AllOrder;
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
                    else
                    {

                        return View(orderVM);
                    }
                }
                var ordersList = allOrders.Split(new string[] { "\"\r\n" },StringSplitOptions.None);
                ordersList = ordersList.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                string failedLines = "";
                int lineNum = 0;
                foreach(var order in ordersList)
                {
                    initializeVM(orderVM);
                    lineNum++;
                    try
                    {
                        var orderDetails = order.Split(new string[] { "\t" }, StringSplitOptions.None);
                        orderDetails = orderDetails.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        addAddressDetailsToVM(orderDetails[4], orderVM);
                        orderVM.Orders.ProductId = getProductIdByName(orderDetails[0]);
                        orderVM.Orders.UserNameId = returnUserNameId();
                        orderVM.Orders.StoreNameId = getStoreNameId(orderDetails[1],returnUserNameId());
                        orderVM.Orders.Quantity = Int32.Parse(orderDetails[3]);
                        if (orderVM.Orders.ProductId > 0)
                        {
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
                        PaymentBalance paymentBalance = userBalance(uNameId);
                        if (!paymentBalance.AllowNegativeBalance && paymentBalance.Balance < orderVM.Orders.Cost)
                        {
                            InsufficientFunds = true;
                            ViewBag.success = false;
                            if (failedLines.Length == 0)
                            {
                                failedLines = orderVM.Orders.CustName;
                            }
                            else
                            {
                                failedLines = failedLines + "<br />" + orderVM.Orders.CustName;
                            }
                            continue;
                        }
                        if (isStoreAuthenticated(orderVM) && orderVM.Orders.ProductId > 0 &&
                            IsProdavailable(orderVM.Orders.ProductId) &&
                            orderVM.Orders.UsDate <= DateTime.Now && Enumerable.Range(1, 100).Contains(orderVM.Orders.Quantity) &&
                            orderVM.Orders.CustName.Length >0 && orderVM.Orders.CustStreet1.Length > 0 &&
                            Enumerable.Range(5,10).Contains(orderVM.Orders.CustZipCode.Length) &&
                            orderVM.Orders.CustCity.Length > 1 && orderVM.Orders.CustState.Length == 2)
                        {
                            orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                            orderVM.Orders.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                            orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                            _unitOfWork.Order.Add(orderVM.Orders);
                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity );
                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                            updateSellerBalance(orderVM.Orders.Cost);
                            _unitOfWork.Save();
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
                ViewBag.success = true;
                // if(failedLines.Length == 0 )
                //{
                if (InsufficientFunds)
                {
                    ViewBag.InsufficientFunds = InsufficientFunds;
                    if(processedLines == 0)
                    {
                        ViewBag.failed = "Insufficient Funds! No Orders were processed!";
                    }
                    else if(processedLines == 1)
                    {
                        ViewBag.ShowErrInLabel = true;
                        ViewBag.failed = "Insufficient Funds! 1 Order Failed:/n" +
                        failedLines;
                    }
                    else
                    {
                        ViewBag.ShowErrInLabel = true;
                        ViewBag.failed = "Insufficient Funds! Only " + processedLines + " Orders were Processed Successfully!/n" +
                    "/nFailed Orders: /n/n" + failedLines;
                    }
                    ViewBag.success = false;
                }
                else if (failedLines.Length > 0)
                {
                    ViewBag.success = false;
                    ViewBag.InsufficientFunds = false;
                    if (processedLines == 0)
                    {
                        ViewBag.failed = "Pay Attention: An error occured! No Orders were processed!";
                    }
                    else if (processedLines == 1)
                    {
                        ViewBag.ShowErrInLabel = true;
                        ViewBag.failed = "Pay Attention: An error occured! Only One Order was Processed Successfully/n" +
                        "/nFailed Orders:/n/n" + failedLines;
                    }
                    else
                    {
                        ViewBag.ShowErrInLabel = true;
                        ViewBag.failed = "Pay Attention: An error occured Only " + processedLines + " Orders were Processed Successfully/n" +
                     "/nFailed Orders:/n/n" + failedLines;
                    }
                    
                }
                else
                {
                    ViewBag.InsufficientFunds = false;
                    ViewBag.failed = "";
                    ViewBag.success = true;
                }
                ViewBag.ProcessedLines = processedLines;
                return View(orderVM);

                //}
                //return RedirectToAction(nameof(Index));
            }
            return View(orderVM.Orders);
        }
        public bool IsProdavailable(int ProductId)
        {
            Product prod = _unitOfWork.Product.Get(ProductId);
            if (!prod.AvailableForSellers || prod.OOSForSellers)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool isStoreAuthenticated(OrderVM orderVM)
        {
            return
                _unitOfWork.UserStoreName.GetAll().Any(a => a.UserNameId == returnUserNameId() && a.Id == orderVM.Orders.StoreNameId);
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
        public int getStoreNameId(string storeName, string userNameId)
        {
            return _unitOfWork.UserStoreName.GetAll().Where(a => a.StoreName.Equals(storeName, StringComparison.InvariantCultureIgnoreCase)
            && a.UserNameId == userNameId).Select(a => a.Id).FirstOrDefault();
        }
        public void addAddressDetailsToVM(string orderDetails, OrderVM orderVM)
        {
            var addressDetails = orderDetails.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            addressDetails = addressDetails.Where(x => !string.IsNullOrEmpty(x)).ToArray();
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
            orderVM.Orders.CustZipCode = cityStateZip[cityStateZip.Length-1];
            orderVM.Orders.CustState = cityStateZip[cityStateZip.Length-2];
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
            for(int i=1;i<(cityStateZip.Length-2);i++)
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
            orderVM.Orders.CustStreet2= "";
            orderVM.Orders.CustStreet1 = "";
            orderVM.Orders.CustCity = "";
            orderVM.Orders.CustState = "";
            orderVM.Orders.CustZipCode = "";
            orderVM.Orders.CustPhone= "";
            orderVM.Orders.IsAdmin = false;
            orderVM.Orders.OrderStatus = SD.OrderStatusAccepted;
        }
        [HttpPost]
        public ActionResult DownloadCSV(DateTime fromDate, DateTime toDate)
        {
            string userNameId = returnUserNameId();
            string fileName =
                    DateTime.Now.DayOfWeek + "_HH" + DateTime.Now.Hour + "_MI" + DateTime.Now.Minute + ".csv";
            IEnumerable<Order> orderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId && a.TrackingNumber != null &&
            a.UsDate >= fromDate && a.UsDate <= toDate).
                OrderBy(a => a.Id);
            orderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId);
            orderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId && a.TrackingNumber != null);
            orderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId && a.TrackingNumber != null && a.UsDate >= fromDate);
            orderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId && a.TrackingNumber != null && 
            a.UsDate >= fromDate && a.UsDate <= toDate);
            StringBuilder sb = new StringBuilder();
            ////Header
            sb.Append("Product,Date,Store,Quantity,Cost,Customer Name,Street1,Street2,City,State,Zip Code,Phone Number,Tracking Number,Carrier");
            sb.Append("\r\n");
                    foreach (Order order in orderList)
                    {
                       sb.Append(returnProductName(order.ProductId)+ ',');
                        sb.Append(order.UsDate.Day + "/" + order.UsDate.Month + "/" + order.UsDate.Year+',');
                        sb.Append(order.StoreName+',');
                        sb.Append(order.Quantity.ToString() + ',');
                sb.Append(order.Cost.ToString()+'$' + ',');
                sb.Append(order.CustName.Replace(",", "").Replace("\"", "") + ',');
                        sb.Append(order.CustStreet1.Replace(",", "").Replace("\"", "") + ',');
                        if (order.CustStreet2 == null)
                        {
                            sb.Append(',');
                        }
                        else
                        {
                            sb.Append(order.CustStreet2.Replace(",", "").Replace("\"", "") + ',');
                       }
                        sb.Append(order.CustCity + ',');
                        sb.Append(order.CustState + ',');
                        sb.Append(order.CustZipCode + ',');
                        if (order.CustPhone != null)
                        {
                            sb.Append(order.CustPhone + ',');
                        }
                        else
                        {
                            sb.Append(',');
                        }
                        sb.Append("=\""+order.TrackingNumber +"\""+ ',');
                        sb.Append(order.Carrier + ',');

                        //Append new line character.
                        sb.Append("\r\n");
                    }
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", fileName);
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Order.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Order.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.Order.Remove(objFromDb);
            updateInventory(objFromDb.ProductId, objFromDb.Quantity*(-1));
            updateSellerBalance(objFromDb.Cost * (-1));
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
        public void updateInventory(int productId, int quantity)
        {
            Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            product.InventoryCount = product.InventoryCount - quantity;
        }
        public void updateSellerBalance(double sellerCost)
        {
            string UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == UNameId).FirstOrDefault();
            paymentBalance.Balance = paymentBalance.Balance - sellerCost;
        }
        public void updateWarehouseBalance(int quantity, int productId)
        {
            Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).FirstOrDefault();
            if (product.OwnByWarehouse)
            {
                paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost_warehouse_items + product.Cost));
            }
            else
            {
                paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost));
            }
        }
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
    }
}
