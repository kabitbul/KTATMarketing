using System;
using System.Collections.Generic;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [Authorize(Roles = SD.Role_Users)]
    public class RefundController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RefundController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string userNameId = returnUserName();
            var refund =_unitOfWork.Refund.GetAll().Join(_unitOfWork.Order.GetAll().
               Where(a => a.UserNameId == userNameId),
                                            refund => refund.OrderId,
                                            order => order.Id,
                                            (refund, order) => new
                                            {
                                                refund
                                            }).Select(a=>a.refund);
            ViewBag.getCustomerName =
               new Func<string, string>(getCustomerName);
            ViewBag.getProductName =
               new Func<string, string>(getProductName);
            ViewBag.getStore =
               new Func<string, string>(getStore);
            ViewBag.getRefundAmount =
               new Func<string, string, string>(getRefundAmount);
            return View(refund);
        }
        public string returnUserName()
        {
            return
                _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id).FirstOrDefault();
        }
        public string getRefundAmount(string OrderId,string quantityRefund)
        {
            Order order = _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(OrderId)).FirstOrDefault();
            double costPerone = order.Cost / order.Quantity;
            return (costPerone * Convert.ToDouble(quantityRefund)).ToString("0.00") + "$";

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
        public string getProductName(string orderId)
        {
            return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(orderId)).Select(a => a.ProductName).FirstOrDefault();
        }
        public string getCustomerName(string orderId)
        {
            return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(orderId)).Select(a => a.CustName).FirstOrDefault();
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
