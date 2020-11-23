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

namespace KTSite.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Authorize(Roles = SD.Role_Warehouse)]
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
            string warehouseUNameId = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).Select(a => a.UserNameId).FirstOrDefault();
            //var PaymentHistory = _unitOfWork.PaymentHistory.getHistoryOfAdminPayment();
            var PaymentHistory = _unitOfWork.PaymentHistory.GetAll().Join(_unitOfWork.PaymentSentAddress.GetAll().Where(a => a.IsAdmin),
                                                         paymentHistory => paymentHistory.SentFromAddressId,
                                                         paymentSentAddress => paymentSentAddress.Id,
                                                         (paymentHistory, paymentSentAddress) => new
                                                         {
                                                             paymentHistory
                                                         }).Select(a => a.paymentHistory).OrderByDescending(a=>a.PayDate);
            return View(PaymentHistory);
        }
        public string getPaymentAddress(int Id)
        {
            return _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == Id).Select(a => a.PaymentTypeAddress).FirstOrDefault();
        }
        public string getPaymentType(int Id)
        {
            return _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == Id).Select(a => a.PaymentType).FirstOrDefault();
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
