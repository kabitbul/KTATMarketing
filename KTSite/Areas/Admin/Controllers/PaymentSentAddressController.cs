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
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            var PaymentSentAddress = _unitOfWork.PaymentSentAddress.GetAll();
            ViewBag.getUserName =
              new Func<string, string>(getUserName);
            ViewBag.getName =
              new Func<string, string>(getName);
            return View(PaymentSentAddress);
        }
        public string getUserName(string userNameId)
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.Id == userNameId).Select(q => q.UserName)).FirstOrDefault();
        }
        public string getName(string userNameId)
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.Id == userNameId).Select(q => q.Name)).FirstOrDefault();
        }
        public IActionResult AddPaymentType(int? Id)
        {
            ViewBag.uNameId = returnUserNameId();
            PaymentSentAddressVM paymentSentAddressVM = new PaymentSentAddressVM()
            {
                PaymentSentAddress = new PaymentSentAddress(),
                paymentType = SD.paymentType
            };
            if (Id != null)
            {
                paymentSentAddressVM.PaymentSentAddress = _unitOfWork.PaymentSentAddress.GetAll().Where(a => a.Id == Id).FirstOrDefault();
            }
            ViewBag.ShowMsg = 0;
            return View(paymentSentAddressVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPaymentType(PaymentSentAddressVM paymentSentAddressVM)
        {
            bool validAddress = Regex.IsMatch(paymentSentAddressVM.PaymentSentAddress.PaymentTypeAddress, SD.MatchEmailPattern); 
            paymentSentAddressVM.paymentType = SD.paymentType;
            if (validAddress)
                ViewBag.validAddress = true;
            else
            {
                ViewBag.validAddress = false;
                ViewBag.ShowMsg = 1;
                return View(paymentSentAddressVM);
            }

                if (ModelState.IsValid)
            {
                if (paymentSentAddressVM.PaymentSentAddress.Id == 0)
                {
                    _unitOfWork.PaymentSentAddress.Add(paymentSentAddressVM.PaymentSentAddress);
                }
                else
                {
                    _unitOfWork.PaymentSentAddress.update(paymentSentAddressVM.PaymentSentAddress);
                }
                    _unitOfWork.Save();
                ViewBag.ShowMsg = 1;
                return View(paymentSentAddressVM);
                //return RedirectToAction(nameof(Index));
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
