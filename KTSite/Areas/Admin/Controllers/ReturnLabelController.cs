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
using System.Globalization;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            var returnLabelList = _unitOfWork.ReturnLabel.GetAll().OrderByDescending(q=>q.Id);
            ViewBag.getCustName =
               new Func<string, string>(returnCustName);
            ViewBag.getUserName =
            new Func<string, string>(returnUserName);
            ViewBag.Refunded = new Func<string, bool>(returnIsRefunded);
            ViewBag.OrderQuantity = new Func<string, int>(returnOrderQuantity);
            ViewBag.IsAdmin = new Func<string, bool>(returnIsAdmin);
            return View(returnLabelList);
        }
        public bool returnIsRefunded(string OrderId)
        {
            return _unitOfWork.Refund.GetAll().Any(a => a.OrderId == Convert.ToInt64(OrderId));
        }
        public bool returnIsAdmin(string OrderId)
        {
            return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(OrderId)).Select(a=>a.IsAdmin).FirstOrDefault();
        }
        public int returnOrderQuantity(string OrderId)
        {
            return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(OrderId)).Select(a => a.Quantity).FirstOrDefault();
        }
        public IActionResult UpdateReturnLabel(long Id)
        {
            ReturnLabel returnLabel = _unitOfWork.ReturnLabel.GetAll().Where(a => a.Id == Id).FirstOrDefault();
                
            ViewBag.ShowMsg = 0;
            ViewBag.failed = false;
            return View(returnLabel);
        }
        
        public string returnCustName(string OrderId)
        {
            return (_unitOfWork.Order.GetAll().Where(q => q.Id == Convert.ToInt64(OrderId)).Select(q => q.CustName)).FirstOrDefault();
        }
        public string returnUserName(string UserNameId)
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.Id == UserNameId).Select(q => q.Name)).FirstOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateReturnLabel(ReturnLabel returnLabel)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");

                    string fileName = rgx.Replace(returnCustName(returnLabel.OrderId.ToString()), "_")+
                         DateTime.Now.Day+"_"+DateTime.Now.Month;
                    var uploads = Path.Combine(webRootPath, @"ReturnLabels");
                    var extention = Path.GetExtension(files[0].FileName);
                    if (returnLabel.FileURL != null)
                    {
                        //this is an edit and we need to remove old file
                        var filePath = Path.Combine(webRootPath, returnLabel.FileURL.TrimStart('\\'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extention), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    returnLabel.FileURL = @"\ReturnLabels\" + fileName + extention;
                }
                ViewBag.ShowMsg = 1;
                _unitOfWork.ReturnLabel.update(returnLabel);
                _unitOfWork.Save();
            }

            ReturnLabel returnLabel2 = _unitOfWork.ReturnLabel.GetAll().Where(a => a.Id == returnLabel.Id).FirstOrDefault();

            return View(returnLabel2);
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
