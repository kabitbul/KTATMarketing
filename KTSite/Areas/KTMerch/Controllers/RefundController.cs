using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class RefundController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RefundController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string uNameId = returnUserNameId();
            var refund = _unitOfWork.Refund.GetAll().Where(a=> a.MerchType == SD.Role_KTMerch && a.MerchId == uNameId);
            ViewBag.getRefundAmount = new Func<string, string, string, string>(returnRefundAmount);
            return View(refund);
        }
        public string returnUserName(string UserNameId)
        {
            return
                _unitOfWork.ApplicationUser.Get(UserNameId).Name;
        }
        public string returnRefundAmount(string Cost, string Quantity,string quantityRefund)
        {
            double costPerone = Convert.ToDouble(Cost) / Convert.ToInt32(Quantity);
            return (costPerone * Convert.ToDouble(quantityRefund)).ToString("0.00") + "$";

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
