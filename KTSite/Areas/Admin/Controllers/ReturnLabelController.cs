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
using Microsoft.AspNetCore.Mvc.Rendering;

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

            ViewBag.ShowMsg = false;
                ViewBag.success = true;
            ViewBag.failed = false;
            return View(returnLabel);
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        public IActionResult AddReturnLabel(long? Id)//Id is Order Id
        {
            string userNameId = returnUserNameId();
            ViewBag.UserNameId = userNameId;
            ReturnLabelVM returnLabelVM = new ReturnLabelVM()
            {
                returnLabel = new ReturnLabel(),
                OrderList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId &&
                                              a.OrderStatus == SD.OrderStatusDone &&
                                       !_unitOfWork.Complaints.GetAll().Any(p => p.OrderId == a.Id)).
                      Select(i => new SelectListItem
                      {
                          Text = i.Id + "-" + i.CustName + "-Quantity: " + i.Quantity,
                          Value = i.Id.ToString()
                      })
            };
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            if (Id != null)
            {
                returnLabelVM.returnLabel.OrderId = (long)Id;
            }
            returnLabelVM.returnLabel.UserNameId = userNameId;
            ViewBag.InsufficientFunds = false;
            return View(returnLabelVM);
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
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
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
                _unitOfWork.ReturnLabel.update(returnLabel);
                _unitOfWork.Save();
                ViewBag.success = true;
            }

            ReturnLabel returnLabel2 = _unitOfWork.ReturnLabel.GetAll().Where(a => a.Id == returnLabel.Id).FirstOrDefault();

            return View(returnLabel2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReturnLabel(ReturnLabelVM returnLabelVM)
        {
            string userNameId = returnUserNameId();
            ViewBag.InvalidQuantity = false;
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            ViewBag.returnExMerchOnly = false;
            ReturnLabelVM returnLabelVM2 = new ReturnLabelVM()
            {

                returnLabel = new ReturnLabel(),
                OrderList = _unitOfWork.Order.GetAll().Where(i => i.UserNameId == userNameId).Select(i => new SelectListItem
                {
                    Text = i.Id + "-" + i.CustName + "-Quantity: " + i.Quantity,
                    Value = i.Id.ToString()
                })
            };
            returnLabelVM2.returnLabel.UserNameId = userNameId;
            if (ModelState.IsValid)
            {
                Order ord = _unitOfWork.Order.Get(returnLabelVM.returnLabel.OrderId);
                returnLabelVM.returnLabel.MerchId = ord.MerchId;
                returnLabelVM.returnLabel.MerchType = ord.MerchType;
                int quantity = _unitOfWork.Order.GetAll().Where(a => a.Id == returnLabelVM.returnLabel.OrderId).Select(a => a.Quantity).FirstOrDefault();
                if (returnLabelVM.returnLabel.ReturnQuantity > quantity)
                {
                    ViewBag.InvalidQuantity = true;
                }
                else
                {
                    //ADD SHIPPING COST TO EXTERNAL FOR THE LABEL
                    //PaymentBalanceMerch paymentBalanceMerch = _unitOfWork.PaymentBalanceMerch.GetAll().
                      //    Where(a => a.UserNameId == returnLabelVM.returnLabel.MerchId).
                        //FirstOrDefault();
                    if (returnLabelVM.returnLabel.MerchType != SD.Role_ExMerch)
                    {
                        ViewBag.ShowMsg = true;
                        ViewBag.InvalidQuantity = false;
                        ViewBag.returnExMerchOnly = true;
                        return View(returnLabelVM2);
                    }
                    //paymentBalanceWarehouse.Balance = paymentBalanceWarehouse.Balance - SD.shipping_cost;
                   //***Amount added to merch only once he add the label.**********
                    // paymentBalanceMerch.Balance = paymentBalanceMerch.Balance + SD.shipping_cost_for_return;
                    ViewBag.InvalidQuantity = false;
                    _unitOfWork.ReturnLabel.Add(returnLabelVM.returnLabel);
                    ViewBag.ReturnCost = SD.shipping_cost_for_return;
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
            }            
            return View(returnLabelVM2);
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
