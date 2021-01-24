using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class PaymentBalanceBackupController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentBalanceBackupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           var userNameIdList = _unitOfWork.paymentBalanceBackup.GetAll().OrderByDescending(a=>a.BackupDate);
            ViewBag.getUserName =
              new Func<string, string>(getUserName);
            ViewBag.getName =
            new Func<string, string>(getName);
           // ViewBag.IsUserRole =  new Func<int, bool>(IsUserRole); 
            return View(userNameIdList);

        }
        public string getUserName(string userNameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == userNameId).Select(a => a.UserName).FirstOrDefault();
        }
        public string getName(string userNameId)
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.Id == userNameId).Select(q => q.Name)).FirstOrDefault();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.PaymentBalance.GetAll();
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.PaymentBalance.GetAll().Where(a => a.Id == id).FirstOrDefault() ;
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.PaymentBalance.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull" });
        }

        #endregion
    }
}
