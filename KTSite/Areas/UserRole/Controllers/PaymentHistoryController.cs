using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [Authorize(Roles = SD.Role_Users)]
    public class PaymentHistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentHistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
              ViewBag.getPaymentAddress =
                  new Func<int, string>(getPaymentAddress);
              ViewBag.getPaymentType =
                new Func<int, string>(getPaymentType);
            string UNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            var PaymentHistory = _unitOfWork.PaymentHistory.GetAll().Where(a => a.UserNameId == UNameId).OrderByDescending(a=>a.PayDate);
            return View(PaymentHistory);
        }
        public IActionResult AddPayment()
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.uNameId = uNameId;
            PaymentHistoryVM paymentHistoryVM = new PaymentHistoryVM()
            {
                PaymentHistory = new PaymentHistory(),
                PaymentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a=> a.UserNameId == uNameId && a.PaymentType == SD.PaymentPayoneer).Select(i => new SelectListItem
                {
                    Text = i.PaymentTypeAddress,
                    Value = i.Id.ToString()
                })
            };

            ViewBag.AlreadyApprovedOrRejected = false;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.payoneermsg = false;
            return View(paymentHistoryVM);
        }
        public IActionResult UpdatePayment(int Id)
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            ViewBag.uNameId = uNameId;
            PaymentHistoryVM paymentHistoryVM = new PaymentHistoryVM()
            {
                PaymentHistory = new PaymentHistory(),
                PaymentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.UserNameId == uNameId).Select(i => new SelectListItem
                {
                    Text = i.PaymentTypeAddress + " - " + i.PaymentType,
                    Value = i.Id.ToString()
                })
            };
            paymentHistoryVM.PaymentHistory = _unitOfWork.PaymentHistory.Get(Id);
            if(paymentHistoryVM.PaymentHistory.Status == SD.PaymentStatusPending)
                ViewBag.AlreadyApprovedOrRejected = false;
            else
                ViewBag.AlreadyApprovedOrRejected = true;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.payoneermsg = false;
            return View(paymentHistoryVM);
        }
        public string getPaymentAddress(int Id)
        {
            return _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == Id).Select(a => a.PaymentTypeAddress).FirstOrDefault();
        }
        public string getPaymentType(int Id)
        {
            return _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == Id).Select(a => a.PaymentType).FirstOrDefault();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPayment(PaymentHistoryVM paymentHistoryVM)
        {
            ViewBag.AlreadyApprovedOrRejected = false;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            paymentHistoryVM.PaymentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.UserNameId == uNameId).Select(i => new SelectListItem
            {
                Text = i.PaymentTypeAddress + " - " + i.PaymentType,
                Value = i.Id.ToString()
            });
            if (ModelState.IsValid)
            {
                //if a new user, create payment balance 
                PaymentBalance pb =  _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == uNameId).FirstOrDefault();
                if (pb == null)
                {
                    PaymentBalance paymentBalance = new PaymentBalance();
                    paymentBalance.UserNameId = uNameId;
                    paymentBalance.Balance = 0;
                    paymentBalance.IsWarehouseBalance = false;
                    paymentBalance.AllowNegativeBalance = false;
                    _unitOfWork.PaymentBalance.Add(paymentBalance);
                    _unitOfWork.Save();
                }
                PaymentSentAddress paymentSentAddress =
                 _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == paymentHistoryVM.PaymentHistory.SentFromAddressId).FirstOrDefault();
                if (paymentSentAddress.PaymentType == SD.PaymentPaypal)//then fees will apply
                {
                    paymentHistoryVM.PaymentHistory.Amount = paymentHistoryVM.PaymentHistory.Amount - SD.paypalOneTimeFee -
                        (paymentHistoryVM.PaymentHistory.Amount * SD.paypalPercentFees / 100);
                }
                 _unitOfWork.PaymentHistory.Add(paymentHistoryVM.PaymentHistory);
                 _unitOfWork.Save();
                ViewBag.success = true;
            }
            return View(paymentHistoryVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePayment(PaymentHistoryVM paymentHistoryVM)
        {
            ViewBag.AlreadyApprovedOrRejected = false;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            paymentHistoryVM.PaymentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.UserNameId == uNameId).Select(i => new SelectListItem
            {
                Text = i.PaymentTypeAddress + " - " + i.PaymentType,
                Value = i.Id.ToString()
            });
            if (ModelState.IsValid)
            {
                PaymentHistory phFromDb = _unitOfWork.PaymentHistory.Get(paymentHistoryVM.PaymentHistory.Id);
                if (phFromDb.Status == SD.PaymentStatusPending)
                {
                    ViewBag.AlreadyApprovedOrRejected = false;
                    PaymentSentAddress paymentSentAddress =
                     _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == paymentHistoryVM.PaymentHistory.SentFromAddressId).FirstOrDefault();
                    if (paymentSentAddress.PaymentType == SD.PaymentPaypal)//then fees will apply
                    {
                        paymentHistoryVM.PaymentHistory.Amount = paymentHistoryVM.PaymentHistory.Amount - SD.paypalOneTimeFee -
                            (paymentHistoryVM.PaymentHistory.Amount * SD.paypalPercentFees / 100);
                    }
                    _unitOfWork.PaymentHistory.update(paymentHistoryVM.PaymentHistory);
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
                else
                    ViewBag.AlreadyApprovedOrRejected = true;
            }
            return View(paymentHistoryVM);
        }
        [HttpPost]
        public JsonResult UpdateBalance(double amount)
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
            PaymentHistory paymentHistory = new PaymentHistory();
            paymentHistory.UserNameId = uNameId;
            paymentHistory.Amount = amount*0.97;
            paymentHistory.PayDate = DateTime.Now;
            paymentHistory.SentFromAddressId = 0;
            paymentHistory.Status = SD.PaymentStatusApproved;

            PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == paymentHistory.UserNameId).FirstOrDefault();
            paymentBalance.Balance = paymentBalance.Balance + (paymentHistory.Amount*0.97);
            _unitOfWork.Save();
            _unitOfWork.PaymentHistory.Add(paymentHistory);
            _unitOfWork.Save();
            
            return Json("success");
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.PaymentHistory.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.PaymentHistory.Get(id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.PaymentHistory.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
