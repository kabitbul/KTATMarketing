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

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [Authorize(Roles = SD.Role_Users)]
    public class ReturnLabelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReturnLabelController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string userNameId = returnUserNameId();
            var returnLabelList = _unitOfWork.ReturnLabel.GetAll().Where(a=>a.UserNameId == userNameId).OrderByDescending(q=>q.Id);
            ViewBag.getCustName =
               new Func<string, string>(returnCustName);
            ViewBag.Refunded = new Func<string, bool>(returnIsRefunded);
            ViewBag.OrderQuantity = new Func<string, int>(returnOrderQuantity);
            ViewBag.HasLabel = new Func<string, bool>(HasLabel);
            return View(returnLabelList);
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        public IActionResult AddReturnLabel(long? Id)//Id is Order Id
        {
            string userNameId = returnUserNameId();
            ViewBag.UserNameId = userNameId;
            ReturnLabelVM returnLabelVM = new ReturnLabelVM()
            {
                returnLabel = new ReturnLabel(),
                OrderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId &&
                                              a.OrderStatus == SD.OrderStatusDone &&
                                       !_unitOfWork.Complaints.GetAll().Any(p => p.OrderId == a.Id)).
                      Select(i => new SelectListItem
                      //OrderList = _unitOfWork.ReturnLabel.getAllOrdersOfUser(returnUserNameId()).Select(i => new SelectListItem
                      {
                    Text = i.Id + "-" + i.CustName + "-Quantity: " + i.Quantity,
                    Value = i.Id.ToString()
                })
            };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            if (Id != null)
            {
                returnLabelVM.returnLabel.OrderId = (long)Id;
            }
            returnLabelVM.returnLabel.UserNameId = userNameId;
            ViewBag.InsufficientFunds = false;
            return View(returnLabelVM);
        }
        public IActionResult UpdateReturnLabel(int Id)
        {
            string userNameId = returnUserNameId();
            ViewBag.UserNameId = userNameId;
            ReturnLabelVM returnLabelVM = new ReturnLabelVM()
            {
                returnLabel = _unitOfWork.ReturnLabel.GetAll().Where(a=>a.Id == Id).FirstOrDefault(),
                //OrderList = _unitOfWork.ReturnLabel.getAllOrdersOfUser(returnUserNameId()).Select(i => new SelectListItem
                OrderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId &&
                                             a.OrderStatus == SD.OrderStatusDone &&
                                      !_unitOfWork.Complaints.GetAll().Any(p => p.OrderId == a.Id)).
                      Select(i => new SelectListItem
                      {
                    Text = i.Id + "-" + i.CustName + "-Quantity: " + i.Quantity,
                    Value = i.Id.ToString()
                })
            };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            ViewBag.InsufficientFunds = false;
            return View(returnLabelVM);
        }
        public string returnCustName(string OrderId)
        {
            return (_unitOfWork.Order.GetAll().Where(q => q.Id == Convert.ToInt64(OrderId)).Select(q => q.CustName)).FirstOrDefault();
        }
        //if the order of the return has a record in refunds
        public bool returnIsRefunded(string OrderId)
        {
            return _unitOfWork.Refund.GetAll().Any(a => a.OrderId == Convert.ToInt64(OrderId));
        }
        public bool HasLabel(string ReturnLabelId)
        {
            string url = _unitOfWork.ReturnLabel.GetAll().Where(a => a.Id == Convert.ToInt32(ReturnLabelId)).Select(a => a.FileURL).FirstOrDefault();
            if (url != null)
                return true;
            return false;
        }
        public int returnOrderQuantity(string OrderId)
        {
            return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(OrderId)).Select(a => a.Quantity).FirstOrDefault();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReturnLabel(ReturnLabelVM returnLabelVM)
        {
            string userNameId = returnUserNameId();
            ViewBag.InvalidQuantity = false;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            ReturnLabelVM returnLabelVM2 = new ReturnLabelVM()
            {
                
                returnLabel = new ReturnLabel(),
                OrderList = _unitOfWork.Order.GetAll().Where(i => i.UserNameId == userNameId).Select(i => new SelectListItem
                {
                    Text = i.Id + "-" + i.CustName + "-Quantity: " + i.Quantity,
                    Value = i.Id.ToString()
                })
            };
            returnLabelVM2.returnLabel.UserNameId = userNameId;
            if (ModelState.IsValid)
            {
                int quantity =_unitOfWork.Order.GetAll().Where(a => a.Id == returnLabelVM.returnLabel.OrderId).Select(a => a.Quantity).FirstOrDefault();
                if (returnLabelVM.returnLabel.ReturnQuantity > quantity)
                {
                    ViewBag.InvalidQuantity = true;
                }
                else
                {
                    PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == userNameId).
                        FirstOrDefault();
                    if(paymentBalance.Balance < SD.shipping_cost && !paymentBalance.AllowNegativeBalance)
                    {
                        ViewBag.InsufficientFunds = true;
                        ViewBag.ShowMsg = true;
                        ViewBag.InvalidQuantity = false;
                        return View(returnLabelVM2);
                    }
                    PaymentBalance paymentBalanceWarehouse = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).
                        FirstOrDefault();
                    paymentBalanceWarehouse.Balance = paymentBalanceWarehouse.Balance - SD.shipping_cost;
                    paymentBalance.Balance = paymentBalance.Balance - SD.shipping_cost;
                    ViewBag.InvalidQuantity = false;
                    _unitOfWork.ReturnLabel.Add(returnLabelVM.returnLabel);
                    ViewBag.ReturnCost = SD.shipping_cost;
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
            }
            ViewBag.InsufficientFunds = false;
            return View(returnLabelVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateReturnLabel(ReturnLabelVM returnLabelVM)
        {
            string userNameId = returnUserNameId();
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            ViewBag.deliveredButNoTracking = false;
            ViewBag.InvalidQuantity = false;
            if (ModelState.IsValid)
            {
                int quantity = _unitOfWork.Order.GetAll().Where(a => a.Id == returnLabelVM.returnLabel.OrderId).Select(a => a.Quantity).FirstOrDefault();
                if (returnLabelVM.returnLabel.ReturnDelivered && returnLabelVM.returnLabel.ReturnTracking == null)
                {
                    ViewBag.deliveredButNoTracking = true;
                }
                else if (returnLabelVM.returnLabel.ReturnQuantity > quantity)
                {
                    ViewBag.InvalidQuantity = true;
                }
                else
                {
                    _unitOfWork.ReturnLabel.update(returnLabelVM.returnLabel);
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
            }
            ReturnLabelVM returnLabelVM2 = new ReturnLabelVM()
            {
                returnLabel = new ReturnLabel(),
                OrderList = _unitOfWork.Order.GetAll().Where(i => i.UserNameId == userNameId).Select(i => new SelectListItem
                {
                    Text = i.Id + "-" + i.CustName + "-Quantity: " + i.Quantity,
                    Value = i.Id.ToString()
                })
            };
            returnLabelVM2.returnLabel.UserNameId = userNameId;
            return View(returnLabelVM2);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll().OrderBy(a => a.ProductName);
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.ReturnLabel.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            PaymentBalance payment = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == returnUserNameId()).FirstOrDefault();
            payment.Balance = payment.Balance + SD.shipping_cost;
            _unitOfWork.ReturnLabel.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull, " + SD.shipping_cost +"$ Added Back To Your Balance!" });
        }

        #endregion
    }
}
