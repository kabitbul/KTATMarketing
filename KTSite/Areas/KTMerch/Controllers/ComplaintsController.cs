using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class ComplaintsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComplaintsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string uNameId = returnUserNameId();
            var complaints = _unitOfWork.Complaints.GetAll().Where(a=>a.MerchType == SD.Role_KTMerch && a.MerchId == uNameId);
           // ViewBag.getStore =
             //  new Func<string, string>(getStore);
            //ViewBag.IsAdmin = new Func<string, bool>(returnIsAdmin);
            return View(complaints);
        }
        //public bool returnIsAdmin(string OrderId)
        //{
        //    return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(OrderId)).Select(a => a.IsAdmin).FirstOrDefault();
        //}
       
        public bool returnIsRefunded(string OrderId)
        {
            Refund refund = _unitOfWork.Refund.GetAll().Where(a => a.OrderId == Convert.ToInt64(OrderId)).FirstOrDefault();
            if (refund != null)
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public IActionResult warehouseResp(int[] Ids)
        {
            foreach (int Id in Ids)
            {
                Complaints complaints = _unitOfWork.Complaints.Get(Id);
                if (complaints.WarehouseResponsibility)
                {
                    complaints.WarehouseResponsibility = false;
                }
                else
                {
                    complaints.WarehouseResponsibility = true;
                }
                _unitOfWork.Save();
            }
            return View();
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
