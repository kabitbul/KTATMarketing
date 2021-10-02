using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.ExternalMerch.Controllers
{
    [Area("ExternalMerch")]
    [Authorize(Roles = SD.Role_ExMerch)]
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
            var complaints = _unitOfWork.Complaints.GetAll().Where(a=>a.MerchType == SD.Role_ExMerch && a.MerchId == uNameId);
            return View(complaints);
        }
        public string getUserName(string unameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == unameId).Select(a => a.Name).FirstOrDefault();
        }
        public IActionResult UpdateComplaint(long Id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            string userNameId = returnUserNameId();
            
            //string uNameId = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).Select(a => a.UserNameId).FirstOrDefault();
            ComplaintsVM complaintsVM;
                complaintsVM = new ComplaintsVM()
                {
                    complaints = _unitOfWork.Complaints.Get(Id),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusDone && a.MerchType == SD.Role_ExMerch && a.MerchId == userNameId).
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
            if (complaintsVM.complaints.OrderId == 0)
            {
                complaintsVM.GeneralNotOrderRelated = true;
            }
            else
            {
                complaintsVM.GeneralNotOrderRelated = false;
            }
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
