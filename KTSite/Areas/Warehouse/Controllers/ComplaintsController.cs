﻿using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Authorize(Roles = SD.Role_Warehouse)]
    public class ComplaintsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComplaintsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var complaints = _unitOfWork.Complaints.GetAll().Where(a=>a.WarehouseResponsibility && a.MerchType != SD.Role_ExMerch);
            ViewBag.getStore = new Func<string, string>(getStore);
            return View(complaints);
        }
        public string getUserName(string unameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == unameId).Select(a => a.Name).FirstOrDefault();
        }
        public string getStore(string storeId)
        {
            return _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == Convert.ToInt32(storeId)).Select(a => a.StoreName).FirstOrDefault();
        }
        public IActionResult UpdateComplaint(long Id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            string userNameId = returnUserNameId();
            bool IsAdmin = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).Select(a => a.IsAdmin).FirstOrDefault();
            //string uNameId = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).Select(a => a.UserNameId).FirstOrDefault();
            ComplaintsVM complaintsVM;
                complaintsVM = new ComplaintsVM()
                {
                    complaints = _unitOfWork.Complaints.Get(Id),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusDone).
                     Select(i => new SelectListItem
                     {
                         Text = i.CustName + "- Id: " + i.Id,
                         Value = i.Id.ToString()
                     }),
                    StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.UserNameId == userNameId).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
                };
            ViewBag.IsAdmin = IsAdmin;
            return View(complaintsVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateComplaint(ComplaintsVM complaintsVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                complaintsVM.complaints.HandledBy =
                    (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name)).FirstOrDefault();
                //if tracking number is not empty, complaint is solved
                if(complaintsVM.complaints.NewTrackingNumber != null)
                {
                    complaintsVM.complaints.Solved = true;
                }
                if (complaintsVM.complaints.NewTrackingNumber == null && complaintsVM.complaints.Solved &&
                    complaintsVM.complaints.SolutionDesc == null)
                {
                    ViewBag.ProvideSolution = true;
                }
                else
                {
                    ViewBag.ProvideSolution = false;
                    _unitOfWork.Complaints.update(complaintsVM.complaints);
                    _unitOfWork.Save();
                    ViewBag.success = true;
                }
                //return RedirectToAction(nameof(Index));
            }
            string userNameId = returnUserNameId();
            ComplaintsVM complaintsVM2;
                complaintsVM2 = new ComplaintsVM()
                {
                    complaints = _unitOfWork.Complaints.Get(complaintsVM.complaints.Id),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.IsAdmin).Where(a => a.OrderStatus == SD.OrderStatusDone).
                    Select(i => new SelectListItem
                    {
                        Text = i.CustName + "- Id: " + i.Id,
                        Value = i.Id.ToString()
                    }),
                    StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.UserNameId == userNameId).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
                };
            if (complaintsVM.complaints.OrderId == 0)
            {
                complaintsVM.GeneralNotOrderRelated = true;
            }
            else
            {
                complaintsVM.GeneralNotOrderRelated = false;
            }
            //if resolution is send again and it is changed to solved -
            //remove inventory count AND add record to returningItem Table
            //if(complaintsVM.complaints.TicketResolution == SD.SendAgain && complaintsVM.complaints.Solved)
            //{
            //    Order ord = _unitOfWork.Order.Get((long)complaintsVM.complaints.OrderId);
            //    int prodId = ord.ProductId;
            //    Product pr = _unitOfWork.Product.Get(prodId);
            //    pr.InventoryCount = pr.InventoryCount - ord.Quantity;

            //    ReturningItem rt = new ReturningItem();
            //    rt.ProductId = prodId;
            //    rt.ItemStatus = SD.ReturningItemRemove;
            //    rt.Quantity = ord.Quantity;
            //    rt.DateArrived = DateTime.Now;
            //    rt.CreatedBy = SD.warehouseTicket;
            //    _unitOfWork.ReturningItem.Add(rt);
            //    _unitOfWork.Save();
            //}
            ViewBag.IsAdmin = complaintsVM2.complaints.IsAdmin;
            return View(complaintsVM2);
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
