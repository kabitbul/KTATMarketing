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
    public class PaymentSentAddressController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentSentAddressController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            var PaymentSentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.UserNameId == UNameId);
            return View(PaymentSentAddress);
        }
        public IActionResult AddPaymentType()
        {
            ViewBag.uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            PaymentSentAddressVM paymentSentAddressVM = new PaymentSentAddressVM()
            {
                PaymentSentAddress = new PaymentSentAddress(),
                paymentType = SD.paymentType
            };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            return View(paymentSentAddressVM);
        }
        public IActionResult EditPaymentType(int Id)
        {
            ViewBag.uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            PaymentSentAddressVM paymentSentAddressVM = new PaymentSentAddressVM()
            {
                PaymentSentAddress =
                _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == Id).FirstOrDefault(),
                paymentType = SD.paymentType
            };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            return View(paymentSentAddressVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPaymentType(PaymentSentAddressVM paymentSentAddressVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            paymentSentAddressVM.paymentType = SD.paymentType;
            if (ModelState.IsValid)
            {
                _unitOfWork.PaymentSentAddress.Add(paymentSentAddressVM.PaymentSentAddress);
                _unitOfWork.Save();
                ViewBag.success = true;
                return View(paymentSentAddressVM);
            }
            return View(paymentSentAddressVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPaymentType(PaymentSentAddressVM paymentSentAddressVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            paymentSentAddressVM.paymentType = SD.paymentType;
            if (ModelState.IsValid)
            {
                _unitOfWork.PaymentSentAddress.update(paymentSentAddressVM.PaymentSentAddress);
                _unitOfWork.Save();
                ViewBag.success = true;
            }
            return View(paymentSentAddressVM);
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
