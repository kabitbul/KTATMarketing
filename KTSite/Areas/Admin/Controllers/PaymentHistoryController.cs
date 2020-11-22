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

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            var PaymentHistory =_unitOfWork.PaymentHistory.GetAll().Join(_unitOfWork.PaymentSentAddress.GetAll().Where(a=>!a.IsAdmin),
                                                         paymentHistory => paymentHistory.SentFromAddressId,
                                                         paymentSentAddress => paymentSentAddress.Id,
                                                         (paymentHistory, paymentSentAddress) => new
                                                         {
                                                             paymentHistory
                                                         }).Select(a=>a.paymentHistory);
                //getHistoryOfAllUsersPayment();
            return View(PaymentHistory);
        }
        public IActionResult ShowWarehousePayment()
        {
            ViewBag.getPaymentAddress =
                new Func<int, string>(getPaymentAddress);
            ViewBag.getPaymentType =
              new Func<int, string>(getPaymentType);
            string warehouseUNameId = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).Select(a=>a.UserNameId).FirstOrDefault();
            //var PaymentHistory = _unitOfWork.PaymentHistory.getHistoryOfAdminPayment();
            var PaymentHistory = _unitOfWork.PaymentHistory.GetAll().Join(_unitOfWork.PaymentSentAddress.GetAll().Where(a => a.IsAdmin),
                                                         paymentHistory => paymentHistory.SentFromAddressId,
                                                         paymentSentAddress => paymentSentAddress.Id,
                                                         (paymentHistory, paymentSentAddress) => new
                                                         {
                                                             paymentHistory
                                                         }).Select(a => a.paymentHistory);
            return View(PaymentHistory);
        }
        public IActionResult PayWarehouse()
        {
            ViewBag.showMsg = false;
            ViewBag.success = true;
            //pay warehouse
            // get the user we pay to from payment balance 
            string uNameIdWarehouse = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).Select(a => a.UserNameId).FirstOrDefault();
            ViewBag.uNameIdWarehouse = uNameIdWarehouse;
            PaymentHistoryVM paymentHistoryVM = new PaymentHistoryVM()
            {
                PaymentHistory = new PaymentHistory(),
                //need to retrieve all addresses belong to an admin user
                PaymentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.IsAdmin).Select(i => new SelectListItem
                 {
                     Text = i.PaymentTypeAddress,//type and type address
                     Value = i.Id.ToString()
                 })
            };
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
        public IActionResult ApproveStatus(int[] Ids)
        {
            foreach(int Id in Ids)
            {
                PaymentHistory paymentHistory = _unitOfWork.PaymentHistory.GetAll().Where(a => a.Id == Id).FirstOrDefault();
                paymentHistory.Status = SD.PaymentStatusApproved;
                PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == paymentHistory.UserNameId).FirstOrDefault();
                paymentBalance.Balance = paymentBalance.Balance + paymentHistory.Amount;
                _unitOfWork.Save();
            }
            return View();
        }
        public IActionResult RejectStatus(int[] Ids)
        {
            foreach (int Id in Ids)
            {
                PaymentHistory paymentHistory = _unitOfWork.PaymentHistory.GetAll().Where(a => a.Id == Id).FirstOrDefault();
                paymentHistory.Status = SD.PaymentStatusRejected;
//                PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.UserNameId == paymentHistory.UserNameId).FirstOrDefault();
//                paymentBalance.Balance = paymentBalance.Balance + paymentHistory.Amount;
                _unitOfWork.Save();
            }
            return View();
        }
        public void AddBalanceToWarehouse(double Amount)
        {
            PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).FirstOrDefault();
            paymentBalance.Balance = paymentBalance.Balance + Amount;
        }
        #region API CALLS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PayWarehouse(PaymentHistoryVM paymentHistoryVM)
        {
            //when ModelState.IsValid equal to FALSE
            var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();
            ViewBag.showMsg = true;
            string uNameIdWarehouse = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).Select(a => a.UserNameId).FirstOrDefault();
            paymentHistoryVM.PaymentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.UserNameId == uNameIdWarehouse).Select(i => new SelectListItem
            {
                Text = i.PaymentTypeAddress,
                Value = i.Id.ToString()
            });
            if (ModelState.IsValid)
            {
                _unitOfWork.PaymentHistory.Add(paymentHistoryVM.PaymentHistory);
                AddBalanceToWarehouse(paymentHistoryVM.PaymentHistory.Amount);
                _unitOfWork.Save();
                ViewBag.Success = true;
                return View(paymentHistoryVM);
            }
            ViewBag.Success = false;
            return View(paymentHistoryVM);
        }
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
