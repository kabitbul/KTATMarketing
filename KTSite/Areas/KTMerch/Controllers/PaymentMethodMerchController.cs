using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Collections.Generic;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class PaymentMethodMerchController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentMethodMerchController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            var PaymentMethod = _unitOfWork.paymentMethodMerch.GetAll().Where(a => a.UserNameId == UNameId);
            return View(PaymentMethod);
        }
        public IActionResult AddPaymentType()
        {
            ViewBag.uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            PaymentMethodMerchVM paymentMethodMerchVM = new PaymentMethodMerchVM()
            {
                PaymentMethodMerch = new PaymentMethodMerch(),
                paymentType = SD.paymentType
            };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            return View(paymentMethodMerchVM);
        }
        public IActionResult EditPaymentType(int Id)
        {
            ViewBag.uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            PaymentMethodMerchVM paymentMethodMerchVM = new PaymentMethodMerchVM()
            {
                PaymentMethodMerch =
                _unitOfWork.paymentMethodMerch.GetAll().Where(a => a.Id == Id).FirstOrDefault(),
                paymentType = SD.paymentType
            };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            return View(paymentMethodMerchVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPaymentType(PaymentMethodMerchVM paymentMethodMerchVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            paymentMethodMerchVM.paymentType = SD.paymentType;
            if (ModelState.IsValid)
            {
                // if checked as preferred, unmark the rest
                if (paymentMethodMerchVM.PaymentMethodMerch.PrefferdMethod)
                {
                    string uNameId = returnUserNameId();
                    //List<PaymentMethodMerch> payMethodList = new List<PaymentMethodMerch>();
                    var payMethodList = _unitOfWork.paymentMethodMerch.GetAll().Where(a => a.UserNameId == uNameId);
                    foreach (var payMethod in payMethodList)
                    {
                        payMethod.PrefferdMethod = false;
                        _unitOfWork.Save();
                    }
                }
                _unitOfWork.paymentMethodMerch.Add(paymentMethodMerchVM.PaymentMethodMerch);
                _unitOfWork.Save();
                ViewBag.success = true;
                return View(paymentMethodMerchVM);
            }
            return View(paymentMethodMerchVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPaymentType(PaymentMethodMerchVM paymentMethodMerchVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            paymentMethodMerchVM.paymentType = SD.paymentType;
            if (ModelState.IsValid)
            {
                // if checked as preferred, unmark the rest
                if (paymentMethodMerchVM.PaymentMethodMerch.PrefferdMethod)
                {
                    string uNameId = returnUserNameId();
                    //List<PaymentMethodMerch> payMethodList = new List<PaymentMethodMerch>();
                    var payMethodList = _unitOfWork.paymentMethodMerch.GetAll().Where(a => a.UserNameId == uNameId);
                    foreach (var payMethod in payMethodList)
                    {
                        payMethod.PrefferdMethod = false;
                        _unitOfWork.Save();
                    }
                }
                _unitOfWork.paymentMethodMerch.update(paymentMethodMerchVM.PaymentMethodMerch);
                _unitOfWork.Save();
                ViewBag.success = true;
            }
            return View(paymentMethodMerchVM);
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
