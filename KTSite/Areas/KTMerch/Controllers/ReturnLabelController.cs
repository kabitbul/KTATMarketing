using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Text.RegularExpressions;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class ReturnLabelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ReturnLabelController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            string uNameId = returnUserNameId();
            var returnLabelList = _unitOfWork.ReturnLabel.GetAll().Where(a=> a.MerchType == SD.Role_KTMerch && a.MerchId == uNameId).OrderByDescending(q=>q.Id);
            ViewBag.getCustName =
               new Func<string, string>(returnCustName);
            //ViewBag.getUserName =
            //new Func<string, string>(returnUserName);
            ViewBag.Refunded = new Func<string, bool>(returnIsRefunded);
            ViewBag.OrderQuantity = new Func<string, int>(returnOrderQuantity);
            //ViewBag.IsAdmin = new Func<string, bool>(returnIsAdmin);
            return View(returnLabelList);
        }
        public bool returnIsRefunded(string OrderId)
        {
            return _unitOfWork.Refund.GetAll().Any(a => a.OrderId == Convert.ToInt64(OrderId));
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        
        public int returnOrderQuantity(string OrderId)
        {
            return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(OrderId)).Select(a => a.Quantity).FirstOrDefault();
        }
        
        public string returnCustName(string OrderId)
        {
            return (_unitOfWork.Order.GetAll().Where(q => q.Id == Convert.ToInt64(OrderId)).Select(q => q.CustName)).FirstOrDefault();
        }
 
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll();
            return Json(new { data = allObj });
        }
        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{
        //    var objFromDb = _unitOfWork.Product.Get(id);
        //    if(objFromDb == null)
        //    {
        //        return Json(new { success = false, message = "Error While Deleting" });
        //    }
        //    _unitOfWork.Product.Remove(objFromDb);
        //    _unitOfWork.Save();
        //    return Json(new { success = true, message = "Delete Successfull" });
        //}

        #endregion
    }
}
