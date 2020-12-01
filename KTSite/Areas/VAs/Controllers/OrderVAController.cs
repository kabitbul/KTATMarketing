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
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Converters;

namespace KTSite.Areas.VAs.Controllers
{
    [Area("VAs")]
    [Authorize(Roles = SD.Role_VAs)]
    public class OrderVAController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderVAController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var orderList = _unitOfWork.Order.GetAll().OrderByDescending(q=>q.Id);
            dynamic myModel = new System.Dynamic.ExpandoObject();
            myModel.Order = _unitOfWork.Order.GetAll();
            ViewBag.getProductName =
               new Func<int, string>(returnProductName);
            ViewBag.getStoreName =
               new Func<int, string>(returnStoreName);
            ViewBag.getUserOrAdminName =
               new Func<int, string>(returnUserOrAdminName);
            ViewBag.getCost =
                          new Func<int, double ,double>(returnCost);
            ViewBag.errSaveInProgress = false;
            ViewBag.AllowReturn = new Func<string, bool>(allowRetrun);
            ViewBag.AllowComplaint = new Func<string, bool>(allowComplaint);
            return View(myModel);
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
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).Select(i => new SelectListItem
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
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).Select(i => new SelectListItem
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
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).Select(i => new SelectListItem
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
        public string returnUserOrAdminName(int storeId)
        {
            UserStoreName userStoreName = _unitOfWork.UserStoreName.GetAll().Where(q => q.Id == storeId).FirstOrDefault();
            if (userStoreName.IsAdminStore)
            {
                return "Admin";
            }
            else
            {
                return _unitOfWork.ApplicationUser.GetAll().
                    Where(a => a.Id == userStoreName.UserNameId).Select(a => a.Name).FirstOrDefault();
            }
        }
        public double returnCost(int productId, double quantity)
        {
            double productCost = (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.Cost)).FirstOrDefault();
            return Convert.ToDouble(String.Format("{0:0.00}", (productCost * quantity))); 
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
                orderVM.Orders.Cost = returnCost(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                if(isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now)
                {
                    bool fail = false;
                    try
                    {
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
            OrderVM orderVM2 = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).Select(i => new SelectListItem
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
                if (isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now)
                {
                    int oldQuantity = _unitOfWork.Order.GetAll().Where(a => a.Id == orderVM.Orders.Id)
                        .Select(a => a.Quantity).FirstOrDefault();
                    int oldProductId = _unitOfWork.Order.GetAll().Where(a => a.Id == orderVM.Orders.Id)
                        .Select(a => a.ProductId).FirstOrDefault();
                    string oldStatus = _unitOfWork.Order.GetAll().Where(a => a.Id == orderVM.Orders.Id)
                                    .Select(a => a.OrderStatus).FirstOrDefault();
                    bool fail = false;
                    try
                    {
                        //changed to cancel
                        if(orderVM.Orders.OrderStatus != oldStatus && orderVM.Orders.OrderStatus == SD.OrderStatusCancelled)
                        {
                            updateInventory(oldProductId, oldQuantity*(-1));
                            updateWarehouseBalance(oldQuantity*(-1), oldProductId);
                            // if it's a cancellation - we dont want any change but the cancellation it self
                                orderVM.Orders = _unitOfWork.Order.GetAll().Where(a => a.Id == orderVM.Orders.Id).FirstOrDefault();
                                orderVM.Orders.OrderStatus = SD.OrderStatusCancelled;
                        }
                        //change from cancel
                        else if (orderVM.Orders.OrderStatus != oldStatus && orderVM.Orders.OrderStatus != SD.OrderStatusCancelled && oldStatus == SD.OrderStatusCancelled)
                        {
                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                            updateWarehouseBalance(orderVM.Orders.Quantity,orderVM.Orders.ProductId);
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
            OrderVM orderVM2 = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName).Select(i => new SelectListItem
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
                foreach(var order in ordersList)
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
                        orderVM.Orders.Cost = returnCost(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
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
                        orderVM.Orders.CustName = RemoveDiacritics(orderVM.Orders.CustName).Replace(",", "");
                        orderVM.Orders.CustStreet1 = RemoveDiacritics(orderVM.Orders.CustStreet1).Replace(",", "");
                        if (orderVM.Orders.CustStreet2.Length > 0)
                        {
                            orderVM.Orders.CustStreet2 = RemoveDiacritics(orderVM.Orders.CustStreet2).Replace(",", "");
                        }
                        if (isStoreAuthenticated(orderVM) && orderVM.Orders.ProductId > 0 &&
                            orderVM.Orders.UsDate <= DateTime.Now && Enumerable.Range(1, 100).Contains(orderVM.Orders.Quantity) &&
                            orderVM.Orders.CustName.Length > 0 && orderVM.Orders.CustStreet1.Length > 0 &&
                            Enumerable.Range(5, 10).Contains(orderVM.Orders.CustZipCode.Length) &&
                            orderVM.Orders.CustCity.Length > 1 && orderVM.Orders.CustState.Length == 2)
                        {
                            _unitOfWork.Order.Add(orderVM.Orders);
                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                            _unitOfWork.Save();
                            processedLines++;
                        }
                        else
                        {
                            if (failedLines.Length == 0)
                            {
                                failedLines = orderVM.Orders.CustName;
                            }
                            else
                            {
                                failedLines = failedLines + "," + orderVM.Orders.CustName;
                            }
                        }
                    }
                    catch
                    {
                        if (failedLines.Length == 0)
                        {
                            failedLines = orderVM.Orders.CustName;
                        }
                        else
                        {
                            failedLines = failedLines + "," + orderVM.Orders.CustName;
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
                    if (processedLines == 0)
                    {
                        ViewBag.failed = "Pay Attention: An error occured! No Orders were processed!";
                    }
                    else if (processedLines == 1)
                    {
                        ViewBag.failed = "Pay Attention: An error occured! Only One Order was Processed Successfully!" +
                        "*failed Orders*: " + failedLines;
                    }
                    else
                    {
                        ViewBag.failed = "Pay Attention: An error occured Only " + processedLines + " Orders were Processed Successfully!" +
                     "*failed Orders*: " + failedLines;
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
            return _unitOfWork.Product.GetAll().Where(a => a.ProductName.Equals(productName,StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id).FirstOrDefault();
        }
        public int getStoreNameId(string storeName)
        {
            return _unitOfWork.UserStoreName.GetAll().Where(a => a.StoreName.Equals(storeName, StringComparison.InvariantCultureIgnoreCase))
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
            orderVM.Orders.CustStreet2= "";
            orderVM.Orders.CustPhone= "";
            orderVM.Orders.IsAdmin = true;
            orderVM.Orders.OrderStatus = SD.OrderStatusAccepted;
        }
        public void updateInventory(int productId, int quantity)
        {
            Product product =_unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            product.InventoryCount = product.InventoryCount - quantity;
        }
        public void updateWarehouseBalance(int quantity, int productId)
        {
            Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).FirstOrDefault();
            if (product.OwnByWarehouse)
            {
                paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost + product.Cost));
            }
            else
            {
                paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost));
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
    }
}
