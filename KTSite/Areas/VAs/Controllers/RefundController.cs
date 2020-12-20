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

namespace KTSite.Areas.VAs.Controllers
{
    [Area("VAs")]
    [Authorize(Roles = SD.Role_VAs)]
    public class RefundController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RefundController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var refund = _unitOfWork.Refund.GetAll();
            ViewBag.getStore =
               new Func<string, string>(getStore);
            return View(refund);
        }
        public string getUserName(string unameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == unameId).Select(a => a.Name).FirstOrDefault();
        }
        public string getStore(string orderId)
        {
             int storeId = _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(orderId)).Select(a => a.StoreNameId).FirstOrDefault();
             return _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == storeId).Select(a => a.StoreName).FirstOrDefault();
        }
        public IActionResult AddRefund(long? Id)//orderId
        {

            RefundVM refundVM = new RefundVM();
            string uNameId = "";
            string uName = "";
            
            uNameId = returnUserNameId();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name)).FirstOrDefault();
            if (Id == null)
            {
                refundVM = new RefundVM()
                {
                    refund = new Refund(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusDone && !a.IsAdmin &&
                    !_unitOfWork.Refund.GetAll().Any(q => q.OrderId == a.Id)).
                    Select(i => new SelectListItem
                    {
                        Text = i.CustName + "- Id: " + i.Id + " Quantity: " + i.Quantity ,
                        Value = i.Id.ToString()
                    })
                };
                ViewBag.UName = uName;
                ViewBag.ShowMsg = false;
                ViewBag.success = true;
                ViewBag.failed = false;
                return View(refundVM);
            }
            else
            {
                refundVM = new RefundVM()
                {
                    refund = new Refund(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.Id == Id).
                    Select(i => new SelectListItem
                    {
                        Text = i.CustName + "- Id: " + i.Id + " Quantity: " + i.Quantity ,
                        Value = i.Id.ToString()
                    })
                };
                ReturnLabel returnLabel = _unitOfWork.ReturnLabel.GetAll().Where(a => a.OrderId == Id).FirstOrDefault();
                if (returnLabel != null)
                {
                    refundVM.refund.ReturnId = returnLabel.Id;
                }
                ViewBag.UName = uName;
                ViewBag.ShowMsg = false;
                ViewBag.success = true;
                ViewBag.failed = false;
                return View(refundVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRefund(RefundVM refundVM)
        {
            bool errAmount = false;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                Order order  = _unitOfWork.Order.GetAll().Where(a => a.Id == refundVM.refund.OrderId).FirstOrDefault();
                if(refundVM.refund.FullRefund)
                {
                    refundVM.refund.RefundQuantity = order.Quantity;
                }
                if(refundVM.refund.RefundQuantity > order.Quantity || refundVM.refund.RefundQuantity <= 0)
                {
                    ViewBag.ErrAmount = true;
                    errAmount = true;
                }
                else
                {
                    ViewBag.ErrAmount = false;
                }
                if (!errAmount)
                {
                    bool ownByWarehouse = _unitOfWork.Product.GetAll().Where(a => a.Id == order.ProductId).Select(a=>a.OwnByWarehouse).FirstOrDefault();
                    PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == order.UserNameId).FirstOrDefault();
                    // add refund amount to seller balance
                    double costPerOne = order.Cost / order.Quantity;
                    paymentBalance.Balance = paymentBalance.Balance + costPerOne*refundVM.refund.RefundQuantity;
                    PaymentBalance warehousePaymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).FirstOrDefault();
                    //remove from warehouse if its his product
                    if (ownByWarehouse)
                    {
                        double productCost = _unitOfWork.Product.GetAll().Where(a => a.Id == order.ProductId).Select(a=>a.Cost).FirstOrDefault();
                        warehousePaymentBalance.Balance = warehousePaymentBalance.Balance + refundVM.refund.RefundQuantity * productCost;
                    }
                    //if charge warehouse for shipping
                    if(refundVM.refund.ChargeWarehouse)
                    {
                        int prodId = _unitOfWork.Order.Get(refundVM.refund.OrderId).ProductId;
                        bool ownByWarehouse2 = _unitOfWork.Product.Get(prodId).OwnByWarehouse;
                        if (ownByWarehouse2)
                        {
                            warehousePaymentBalance.Balance = warehousePaymentBalance.Balance + refundVM.refund.RefundQuantity * SD.shipping_cost_warehouse_items;
                        }
                        else
                        {
                            warehousePaymentBalance.Balance = warehousePaymentBalance.Balance + refundVM.refund.RefundQuantity * SD.shipping_cost;
                        }
                    }
                    if (refundVM.refund.RefundQuantity == order.Quantity)
                    {
                        order.OrderStatus = SD.OrderStatusFullRefund;
                    }
                    else
                    {
                        order.OrderStatus = SD.OrderStatusPartialRefund;
                    }
                    _unitOfWork.Order.update(order);
                    _unitOfWork.Refund.Add(refundVM.refund);
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
                //return RedirectToAction(nameof(Index));
            }
            RefundVM refundVM2 = new RefundVM()
            {
                refund = new Refund(),
                OrdersList = _unitOfWork.Order.GetAll().Where(a => a.Id == refundVM.refund.OrderId).
                 Select(i => new SelectListItem
                {
                    Text = i.CustName + "- Id: " + i.Id + " Quantity: " + i.Quantity ,
                    Value = i.Id.ToString()
                })
             };
            return View(refundVM2);
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
