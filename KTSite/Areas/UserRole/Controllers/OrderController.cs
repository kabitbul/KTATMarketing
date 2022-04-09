using ExcelDataReader;
using KTSite.DataAccess.Data;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [Authorize(Roles = SD.Role_Users)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public OrderController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
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
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && !a.OOSForSellers && ((uNameId == SD.Kfir_Buyer) ||
                                         (uNameId != SD.Kfir_Buyer && a.MerchId != SD.Kfir_Merch))).
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
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.InsufficientFunds = false;
            ViewBag.failed = false;
            ViewBag.hideAddBtn = false;
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
                Orders = _unitOfWork.Order.GetAll().Where(a => a.Id == id).FirstOrDefault(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && !a.OOSForSellers && ((uNameId == SD.Kfir_Buyer) ||
                                         (uNameId != SD.Kfir_Buyer && a.MerchId != SD.Kfir_Merch))).
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
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && !a.OOSForSellers && ((uNameId == SD.Kfir_Buyer) ||
                                         (uNameId != SD.Kfir_Buyer && a.MerchId != SD.Kfir_Merch))).
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
        public IActionResult AddOrdersShopsFB()
        {
            string uNameId = "";
            string uName = "";
            ApplicationUser appUser = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).FirstOrDefault());
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            ExcelUploadsForShopsVM excelUploadsForShopsVM = new ExcelUploadsForShopsVM()
            {
                excelUploadsForShops = new ExcelUploadsForShops(),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(q => q.UserNameId == appUser.Id && !_unitOfWork.excelUploadsForShops.GetAll().
                                                                       Any(a => a.UserId == appUser.Id && a.StoreId == q.Id && !a.TrackingUpdated))
                .Select(i => new SelectListItem
                {
                    Text = i.StoreName,
                    Value = i.Id.ToString()
                }),
                StoresListTR = _unitOfWork.UserStoreName.GetAll().Where(q => q.UserNameId == appUser.Id && _unitOfWork.excelUploadsForShops.GetAll().
                                                                       Any(a => a.UserId == appUser.Id && a.StoreId == q.Id && !a.TrackingUpdated))
                .Select(i => new SelectListItem
                {
                    Text = i.StoreName+"-fromOrdId-"+ i.FromOrdId + "-toOrdId-"+i.ToOrdId,
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
            string uNameId = returnUserNameId();
            if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
            {
                return Convert.ToDouble(String.Format("{0:0.00}", ((productCost + SD.FBMP_FEE) * quantity)));
            }
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
            orderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId).ToList();
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
            ViewBag.hideAddBtn = true;
            PaymentBalance paymentBalance = userBalance(uNameId);
            OrderVM orderVM2 = new OrderVM()
            {
                Orders = new Order(),
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && !a.OOSForSellers && ((uNameId == SD.Kfir_Buyer) ||
                                         (uNameId != SD.Kfir_Buyer && a.MerchId != SD.Kfir_Merch))).
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
                if (paymentBalance == null || (!paymentBalance.AllowNegativeBalance && paymentBalance.Balance < orderVM.Orders.Cost))
                {
                    ViewBag.InsufficientFunds = true;
                    ViewBag.failed = false;
                    return View(orderVM2);
                }
                ViewBag.InsufficientFunds = false;
                if (isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now &&
                    ((uNameId == SD.Kfir_Buyer) || (orderVM.Orders.MerchId != SD.Kfir_Merch)))
                {
                    Order lastOrd = _unitOfWork.Order.GetAll().OrderByDescending(a => a.Id).Take(1).FirstOrDefault();
                    if (lastOrd.CustName != orderVM.Orders.CustName ||
                        lastOrd.ProductId != orderVM.Orders.ProductId ||
                        lastOrd.CustStreet1 != orderVM.Orders.CustStreet1 ||
                        lastOrd.CustZipCode != orderVM.Orders.CustZipCode ||
                        lastOrd.Quantity != orderVM.Orders.Quantity)
                    {
                        Product pr = _unitOfWork.Product.Get(orderVM.Orders.ProductId);
                        orderVM.Orders.ProductName = pr.ProductName;//returnProductName(orderVM.Orders.ProductId);
                        orderVM.Orders.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                        orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                        orderVM.Orders.CustName = orderVM.Orders.CustName.Trim();
                        orderVM.Orders.CustZipCode = orderVM.Orders.CustZipCode.Trim();
                        if (pr.MerchId != null)
                        {
                            orderVM.Orders.MerchId = pr.MerchId;
                            orderVM.Orders.MerchType = pr.MerchType;
                        }
                        using (var dbContextTransaction = _db.Database.BeginTransaction())
                        {
                            _unitOfWork.Order.Add(orderVM.Orders);
                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                            if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
                            {
                                updateSellerBalance(orderVM.Orders.Cost + (SD.FBMP_FEE * orderVM.Orders.Quantity));
                            }
                            else
                            {
                                updateSellerBalance(orderVM.Orders.Cost);
                            }
                            //updateSellerBalance(orderVM.Orders.Cost);
                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                            //_unitOfWork.Save();
                            _db.SaveChanges();
                            dbContextTransaction.Commit();
                        }
                    }
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
                ProductList = _unitOfWork.Product.GetAll().Where(a => a.AvailableForSellers && !a.OOSForSellers && ((uNameId == SD.Kfir_Buyer) ||
                                         (uNameId != SD.Kfir_Buyer && a.MerchId != SD.Kfir_Merch))).
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
                if (isStoreAuthenticated(orderVM) && orderVM.Orders.UsDate <= DateTime.Now &&
                    ((uNameId == SD.Kfir_Buyer) || (orderVM.Orders.MerchId != SD.Kfir_Merch)))
                {
                    using (var dbContextTransaction = _db.Database.BeginTransaction())
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
                                if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
                                {
                                    updateSellerBalance(oldCost * (-1) - (SD.FBMP_FEE * oldQuantity));
                                }
                                else
                                {
                                    updateSellerBalance(oldCost * (-1));
                                }
                                //updateSellerBalance(oldCost * (-1));
                                // if it's a cancellation - we dont want any change but the cancellation it self
                                orderVM.Orders = _unitOfWork.Order.GetAll().Where(a => a.Id == orderVM.Orders.Id).FirstOrDefault();
                                orderVM.Orders.OrderStatus = SD.OrderStatusCancelled;
                            }
                            //change from cancel
                            else if (orderVM.Orders.OrderStatus != oldStatus && orderVM.Orders.OrderStatus != SD.OrderStatusCancelled && oldStatus == SD.OrderStatusCancelled)
                            {
                                updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                                updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                                if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
                                {
                                    updateSellerBalance(orderVM.Orders.Cost + (orderVM.Orders.Quantity * SD.FBMP_FEE));
                                }
                                else
                                {
                                    updateSellerBalance(orderVM.Orders.Cost);
                                }
                                //updateSellerBalance(orderVM.Orders.Cost);

                            }
                            //status didnt change
                            else if (orderVM.Orders.OrderStatus == oldStatus && orderVM.Orders.OrderStatus != SD.OrderStatusCancelled)
                            {
                                if (oldQuantity != orderVM.Orders.Quantity)
                                {
                                    updateInventory(orderVM.Orders.ProductId, (orderVM.Orders.Quantity - oldQuantity));
                                    updateWarehouseBalance((orderVM.Orders.Quantity - oldQuantity), orderVM.Orders.ProductId);
                                    if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
                                    {
                                        orderVM.Orders.Cost = orderVM.Orders.Cost + (SD.FBMP_FEE * (orderVM.Orders.Quantity - oldQuantity));
                                    }

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
                            //_unitOfWork.Save();
                            ViewBag.success = true;
                            _db.SaveChanges();
                            dbContextTransaction.Commit();

                        }
                        ViewBag.failed = false;
                    }
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
            if (orderId != null)
            {
                Order order = _unitOfWork.Order.Get(Convert.ToInt64(orderId));
                order.OrderStatus = SD.OrderStatusCancelled;
                order.ProductName = returnProductName(order.ProductId);
                order.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                order.StoreName = returnStoreName(order.StoreNameId);
                _unitOfWork.Order.update(order);
                updateInventory(order.ProductId, (order.Quantity * (-1)));
                updateWarehouseBalance((order.Quantity * (-1)), order.ProductId);
                string uNameId = returnUserNameId();
                if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
                {
                    updateSellerBalance((order.Cost * (-1)) - (SD.FBMP_FEE * order.Quantity));
                }
                else
                {
                    updateSellerBalance((order.Cost * (-1)));
                }
                //updateSellerBalance((order.Cost * (-1)));
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
                var ordersList = allOrders.Split(new string[] { "\"\r\n" }, StringSplitOptions.None);
                ordersList = ordersList.Where(x => !string.IsNullOrEmpty(x)).ToArray();
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
                        orderDetails = orderDetails.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        addAddressDetailsToVM(orderDetails[4], orderVM);
                        orderVM.Orders.ProductId = getProductIdByName(orderDetails[0]);
                        orderVM.Orders.UserNameId = returnUserNameId();
                        orderVM.Orders.StoreNameId = getStoreNameId(orderDetails[1], returnUserNameId());
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
                        PaymentBalance paymentBalance = userBalance(uNameId);
                        if (paymentBalance == null || (!paymentBalance.AllowNegativeBalance && paymentBalance.Balance < orderVM.Orders.Cost))
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
                            orderVM.Orders.CustName.Length > 0 && orderVM.Orders.CustStreet1.Length > 0 &&
                            Enumerable.Range(5, 10).Contains(orderVM.Orders.CustZipCode.Length) &&
                            orderVM.Orders.CustCity.Length > 1 && orderVM.Orders.CustState.Length == 2 &&
                            ((uNameId == SD.Kfir_Buyer) || (orderVM.Orders.MerchId != SD.Kfir_Merch)))
                        {
                            orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                            orderVM.Orders.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                            orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                            if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
                            {
                                orderVM.Orders.Cost = orderVM.Orders.Cost + (orderVM.Orders.Quantity * SD.FBMP_FEE);
                            }
                            _unitOfWork.Order.Add(orderVM.Orders);
                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                            if (uNameId == SD.FBMP_USER_HAY || uNameId == SD.FBMP_USER_BENNY)
                            {
                                updateSellerBalance(orderVM.Orders.Cost + (orderVM.Orders.Quantity * SD.FBMP_FEE));
                            }
                            else
                            {
                                updateSellerBalance(orderVM.Orders.Cost);
                            }
                                //updateSellerBalance(orderVM.Orders.Cost);
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
                        string addLine = addToFailedLines(order);
                        if (failedLines.Length == 0)
                        {
                            failedLines = addLine;
                        }
                        else
                        {
                            failedLines = failedLines + "/n" + addLine;
                        }
                    }
                }
                }
                ViewBag.success = true;
                // if(failedLines.Length == 0 )
                //{
                if (InsufficientFunds)
                {
                    ViewBag.InsufficientFunds = InsufficientFunds;
                    if (processedLines == 0)
                    {
                        ViewBag.failed = "Insufficient Funds! No Orders were processed!";
                    }
                    else if (processedLines == 1)
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
        public string addToFailedLines(string order)
        {
            //get location of first " 
            int locStart = order.IndexOf("\"");
            int locEnd = order.IndexOf("\r", locStart);
            int locSize;
            if (locEnd > locStart)
            {
                locSize = locEnd - locStart;
            }
            else
            {
                locSize = 0;
            }

            if (locSize > 0)
                return order.Substring(locStart + 1, (locEnd - locStart) - 1);
            else
                return order.Substring(locStart);

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
            if (addressDetails.Length == 4)
            {
                cityStateZipToVM(addressDetails[2], orderVM);
            }
            else if (addressDetails.Length == 5)
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
            orderVM.Orders.IsAdmin = false;
            orderVM.Orders.OrderStatus = SD.OrderStatusAccepted;
            orderVM.Orders.MerchType = "";
            orderVM.Orders.MerchId = "";
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
                sb.Append(returnProductName(order.ProductId) + ',');
                sb.Append(order.UsDate.Day + "/" + order.UsDate.Month + "/" + order.UsDate.Year + ',');
                sb.Append(order.StoreName + ',');
                sb.Append(order.Quantity.ToString() + ',');
                sb.Append(order.Cost.ToString() + '$' + ',');
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
                sb.Append("=\"" + order.TrackingNumber + "\"" + ',');
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
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.Order.Remove(objFromDb);
            updateInventory(objFromDb.ProductId, objFromDb.Quantity * (-1));
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
            PaymentBalance warehousePaymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).FirstOrDefault();
            if (product.MerchType == SD.Role_KTMerch)
            {
                //pay warehouse
                //paymentBalance.Balance = paymentBalance.Balance - (quantity * (SD.shipping_cost));
                warehousePaymentBalance.Balance = warehousePaymentBalance.Balance - (quantity * (product.ShippingCharge));
                //pay merch --product cost minus shipping minus feeprecent SD.FeesOfKTMerch
                PaymentBalanceMerch KTMerchPaymentBalance = _unitOfWork.PaymentBalanceMerch.GetAll().Where(a => a.UserNameId == product.MerchId).FirstOrDefault();
                double totalProfit = 0.0;
                double addToMinimum = 0;

                totalProfit = (product.SellersCost * quantity * (1 - SD.FeesOfKTMerch)) - ((product.ShippingCharge) * quantity);
                if ((product.SellersCost * SD.FeesOfKTMerch) < 0.8)
                {
                    addToMinimum = 0.8 - (product.SellersCost * SD.FeesOfKTMerch);
                    totalProfit = totalProfit - (addToMinimum * quantity);
                    totalProfit = Math.Round(totalProfit, 2);
                }
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
                if (!fVM.CSVFile.FileName.ToUpper().EndsWith("XLSX") && !fVM.CSVFile.FileName.ToUpper().EndsWith("XLS"))
                {
                    success = 0;
                    excep = "File must be of type xlsx (Excel file)";
                    return Json(new { excep, success });
                }
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    fVM.CSVFile.CopyTo(stream);
                    stream.Position = 0;
                    long fromOrderId = 0;
                    bool insufficientFunds = false;
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
                                    string prd = skuNamefromExcel.Trim();
                                    Product pr = _unitOfWork.Product.GetAll().Where(a => a.ProductName.Equals(prd, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                    //int prodId = getProductIdByName(skuNamefromExcel);
                                    if (pr != null && pr.Id > 0)
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
                                        ord.ProductId = pr.Id;
                                        ord.ProductName = skuNamefromExcel;
                                        ord.StoreName = _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == fVM.storeId).Select(a => a.StoreName).FirstOrDefault();
                                        ord.UserNameToShow = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(a => a.Name).FirstOrDefault();
                                        ord.Cost = returnCost(pr.Id, ord.Quantity);
                                        
                                        ord.StoreNameId = fVM.storeId;
                                        ord.OrderStatus = SD.OrderStatusAccepted;
                                        ord.UserNameId = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id).FirstOrDefault();
                                        ord.UsDate = DateTime.Now.Date;
                                        if (pr.MerchType == SD.Role_KTMerch)
                                        {
                                            ord.MerchType = SD.Role_KTMerch;
                                            ord.MerchId = pr.MerchId;
                                        }
                                        else if (pr.MerchType == SD.Role_ExMerch)
                                        {
                                            ord.MerchType = SD.Role_ExMerch;
                                            ord.MerchId = pr.MerchId;
                                        }
                                        //balance
                                        PaymentBalance paymentBalance = userBalance(UNameId);
                                        if (paymentBalance == null || (!paymentBalance.AllowNegativeBalance && paymentBalance.Balance < ord.Cost))
                                        {
                                            insufficientFunds = true;
                                            break;
                                        }
                                        OrderVM orderVM = new OrderVM();
                                        orderVM.Orders = ord;
                                    if (isStoreAuthenticated(orderVM) && ord.ProductId > 0 &&
                                        IsProdavailable(orderVM.Orders.ProductId) &&
                                        orderVM.Orders.CustName.Length > 0 && orderVM.Orders.CustStreet1.Length > 0 &&
                                        Enumerable.Range(5, 10).Contains(orderVM.Orders.CustZipCode.Length) &&
                                        orderVM.Orders.CustCity.Length > 1 && orderVM.Orders.CustState.Length == 2 &&
                                       ((UNameId == SD.Kfir_Buyer) || (orderVM.Orders.MerchId != SD.Kfir_Merch)))
                                        {
                                            orderVM.Orders.ProductName = returnProductName(orderVM.Orders.ProductId);
                                            orderVM.Orders.UserNameToShow = _unitOfWork.ApplicationUser.Get(returnUserNameId()).Name;
                                            orderVM.Orders.StoreName = returnStoreName(orderVM.Orders.StoreNameId);
                                            if (UNameId == SD.FBMP_USER_HAY || UNameId == SD.FBMP_USER_BENNY)
                                            {
                                                orderVM.Orders.Cost = orderVM.Orders.Cost + (orderVM.Orders.Quantity * SD.FBMP_FEE);
                                            }
                                            //_db.Orders.Add(ord);
                                            _db.Orders.AddRange(ord);
                                            updateInventory(orderVM.Orders.ProductId, orderVM.Orders.Quantity);
                                            updateWarehouseBalance(orderVM.Orders.Quantity, orderVM.Orders.ProductId);
                                            if (UNameId == SD.FBMP_USER_HAY || UNameId == SD.FBMP_USER_BENNY)
                                            {
                                                updateSellerBalance(orderVM.Orders.Cost + (orderVM.Orders.Quantity * SD.FBMP_FEE));
                                            }
                                            else
                                            {
                                                updateSellerBalance(orderVM.Orders.Cost);
                                            }
                                            _db.SaveChanges();
                                        }
                                    else
                                        {
                                            success = 0;
                                            excep = "Line " + countRec + " Error Occured";
                                            dbContextTransaction.Rollback();
                                            return Json(new { excep, success });
                                        }
                                        
                                        proccessedOrders++;
                                        //_unitOfWork.Save();
                                    }
                                    else if(countRec > 1)// if product not found stop entire process(ignore first 2 lines which is a header
                                    {
                                        success = 0;
                                        excep = "Line " + countRec + " Product not recognized: "+ skuNamefromExcel;
                                        dbContextTransaction.Rollback();

                                        return Json(new { excep, success });
                                     }
                                    countRec++;
                                }
                                catch (Exception ex)
                                {
                                    dbContextTransaction.Rollback();
                                    existFail = true;
                                }
                            }
                            if (insufficientFunds)
                            {
                                success = 0;
                                excep = "Insufficient Funds!";

                                return Json(new { excep, success });
                            }
                            else if (existFail)
                            {
                                success = 0;
                                excep = "Unknwon Error Occured, Transction aborted.";
                            }
                            else
                            {
                                dbContextTransaction.Commit();
                                ExcelUploadsForShops exUp = new ExcelUploadsForShops();
                                long toOrd = _db.Orders.Max(o => o.Id);
                                long fromOrd = 0;
                                exUp.UserId = _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id).FirstOrDefault();
                                IEnumerable<Order> orderList =   _unitOfWork.Order.GetAll().Where(q => q.UsDate.Date == DateTime.Now.Date).OrderByDescending(a=>a.Id);
                                if (orderList.Any())
                                {
                                    int countOrd = 0;
                                    foreach (Order order in orderList)
                                    {

                                        
                                        if(order.StoreNameId == fVM.storeId)
                                        {
                                            countOrd++;
                                        }
                                        if(countOrd == proccessedOrders)
                                        {
                                            //exUp.FromOrderId = order.Id;
                                            fromOrd = order.Id;
                                            break;
                                        }
                                    }
                                }
                                //exUp.FromOrderId = toOrd - (proccessedOrders - 1);
                                exUp.TrackingUpdated = false;
                                exUp.CreatedDate = DateTime.Now; exUp.StoreId = fVM.storeId;
                                exUp.ToOrderId = toOrd;
                                exUp.FromOrderId = fromOrd;
                                _db.ExcelUploadsForShopss.Add(exUp);
                                _unitOfWork.excelUploadsForShops.Add(exUp);
                                _unitOfWork.Save();
                                UserStoreName usStore = _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == fVM.storeId).FirstOrDefault();
                                usStore.FromOrdId = fromOrd;
                                usStore.ToOrdId = toOrd;
                                _unitOfWork.UserStoreName.update(usStore);
                                _unitOfWork.Save();
                                success = 1;
                                excep = proccessedOrders + " Orders Added successfully!";
                                return Json(new { excep, success });
                            }
                        }
                    }
                }
                return Json(new { excep, success });
            }
            catch(Exception ex)
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
            string fileName = storeName+ "_"+DateTime.Now.DayOfWeek + "_HH" + DateTime.Now.Hour + "_MI" + DateTime.Now.Minute + ".csv";
            IEnumerable<Order> orderList =  _unitOfWork.Order.GetAll().Where(a => a.Id >= ex.FromOrderId && a.Id <= ex.ToOrderId).
                                           OrderBy(a => a.Id);
            int missingtrackingCounter = 0;
            StringBuilder sb = new StringBuilder();
            //Header
            int lineCounter = 0;
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (Order order in orderList)
                    {
                        if (excelUploadsForShopsVM.excelUploadsForShops.StoreId == order.StoreNameId)
                        {
                            sb.Append(order.Quantity.ToString());
                            sb.Append(',');
                            sb.Append("USPS");
                            sb.Append(',');
                            sb.Append("=\"");
                            sb.Append(order.TrackingNumber);
                            sb.Append("\"");
                            sb.Append(',');
                            sb.Append(order.CustName);
                            //_unitOfWork.Order.Get(order.Id).OrderStatus = SD.OrderStatusDone;
                            //_unitOfWork.Order.Get(order.Id).TrackingUpdated = true;
                            order.TrackingUpdated = true;
                            //_db.Orders.Update(_unitOfWork.Order.Get(order.Id));
                            _db.Orders.Update(order);
                            sb.Append("\r\n");
                            lineCounter++;
                            if (order.TrackingNumber == "" || order.TrackingNumber == null || order.TrackingNumber.Length < 5)
                            {
                                missingtrackingCounter++;
                            }
                        }
                    }
                    if (missingtrackingCounter > 0)
                    {
                        if (missingtrackingCounter == 1)
                            sb.Insert(0, "********,**********,There is a record without tracking number" + "**********,***********\r\n");
                        else
                            sb.Insert(0, "********,**********,There are " + missingtrackingCounter + " records without tracking number" + "**********,***********\r\n");
                    }

                    //update when file downloaded
                    ex.TrackingUpdated = true;
                    //_unitOfWork.Save();
                    _db.ExcelUploadsForShopss.Update(ex);
                    _db.SaveChanges();
                    dbContextTransaction.Commit();
                    DeleteOldRecFromTable(excelUploadsForShopsVM.excelUploadsForShops.StoreId, ex.Id);
                }
                catch
                {
                    ViewBag.errSaveInProgress = true;
                }
            }
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/csv", fileName);
        }

        public void DeleteOldRecFromTable(int storeId, int Id)
        {

            IEnumerable<ExcelUploadsForShops> exList = _unitOfWork.excelUploadsForShops.GetAll().Where(a => a.StoreId == storeId && a.Id != Id);
            foreach (ExcelUploadsForShops ex in exList)
            {
                _unitOfWork.excelUploadsForShops.Remove(ex);
            }
            _unitOfWork.Save();
        }
    }
}