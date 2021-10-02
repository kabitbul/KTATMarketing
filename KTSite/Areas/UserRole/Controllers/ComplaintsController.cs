using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [Authorize(Roles = SD.Role_Users)]
    public class ComplaintsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComplaintsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string userNameId = returnUserNameId();
            var complaints = _unitOfWork.Complaints.GetAll().Where(a=>a.UserNameId == userNameId);
            ViewBag.Refunded = new Func<string, bool>(returnIsRefunded);
            ViewBag.getStore = new Func<string, string>(getStore);
            return View(complaints);
        }
        public string getStore(string storeId)
        {
            return _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == Convert.ToInt32(storeId)).Select(a => a.StoreName).FirstOrDefault();
        }
        public bool returnIsRefunded(string OrderId)
        {
            Refund refund = _unitOfWork.Refund.GetAll().Where(a => a.OrderId == Convert.ToInt64(OrderId)).FirstOrDefault();
            if (refund != null)
            {
                return true;
            }
            return false;
        }
        public IActionResult AddComplaint(long? Id)
        {
            ComplaintsVM complaintsVM;
            string uNameId = "";
            string uName = "";
            uNameId = returnUserNameId();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            if (Id == null)
            {
                complaintsVM = new ComplaintsVM()
                {
                    complaints = new Complaints(),
                    //OrdersList = _unitOfWork.Complaints.getAllOrdersOfUser(uNameId).Select(i => new SelectListItem
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == uNameId &&
                                              a.OrderStatus == SD.OrderStatusDone &&
                                       !_unitOfWork.Complaints.GetAll().Any(p => p.OrderId == a.Id)).
                      Select(i => new SelectListItem
                      {
                        Text = i.CustName + "- Id: " + i.Id,
                        Value = i.Id.ToString()
                    }),
                    StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.UserNameId == uNameId).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    }),
                    TicketResolutionList = SD.TicketResolution
                };
            }
            else
            {
                complaintsVM = new ComplaintsVM()
                {
                    complaints = new Complaints(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a=> a.Id == Id).Select(i => new SelectListItem
                    {
                        Text = i.CustName + "- Id: " + i.Id,
                        Value = i.Id.ToString()
                    }),
                    StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.UserNameId == uNameId).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    }),
                    TicketResolutionList = SD.TicketResolution
                };
                complaintsVM.complaints.OrderId = Id;
            }

                ViewBag.UNameId = uNameId;
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            return View(complaintsVM);
        }
        public IActionResult UpdateComplaint(long Id)
        {
            ComplaintsVM complaintsVM;
            string uNameId = "";
            string uName = "";
            uNameId = returnUserNameId();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();

            complaintsVM = new ComplaintsVM()
            {
                complaints = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).FirstOrDefault(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == uNameId).Select(i => new SelectListItem
                    {
                        Text = i.CustName + "- Id: " + i.Id,
                        Value = i.Id.ToString()
                    }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.UserNameId == uNameId).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    }),
                TicketResolutionList = SD.TicketResolution
            };
            if (complaintsVM.complaints.OrderId == 0)
            {
                complaintsVM.GeneralNotOrderRelated = true;
            }
            else
            {
                complaintsVM.GeneralNotOrderRelated = false;
            }
            ViewBag.UNameId = uNameId;
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            ViewBag.failed = false;
            return View(complaintsVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComplaint(ComplaintsVM complaintsVM)
        {
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {
                if (complaintsVM.complaints.Id == 0)
                {
                    if (complaintsVM.GeneralNotOrderRelated)
                    {
                        complaintsVM.complaints.OrderId = 0;
                        complaintsVM.complaints.TicketResolution = SD.NotRelevant;
                    }
                    else//if its not a general ticket, get the storeid based on order
                    {
                        complaintsVM.complaints.StoreId =
                            _unitOfWork.Order.GetAll().Where(a => a.Id == complaintsVM.complaints.OrderId).Select(a => a.StoreNameId).
                            FirstOrDefault();
                        Order ord = _unitOfWork.Order.Get((long)complaintsVM.complaints.OrderId);
                        complaintsVM.complaints.ProductName = ord.ProductName;
                        complaintsVM.complaints.CustName = ord.CustName;
                        complaintsVM.complaints.MerchId = ord.MerchId;
                        complaintsVM.complaints.MerchType = ord.MerchType;
                    }
                    _unitOfWork.Complaints.Add(complaintsVM.complaints);
                }
                else
                {
                    if (complaintsVM.GeneralNotOrderRelated)
                    {
                        complaintsVM.complaints.OrderId = 0;
                        //complaintsVM.complaints.StoreId = 0;
                        complaintsVM.complaints.TicketResolution = SD.NotRelevant;
                    }
                    _unitOfWork.Complaints.update(complaintsVM.complaints);
                }
                _unitOfWork.Save();
                ViewBag.success = true;
                //  ViewBag.storeExist = storeExist;


                //return RedirectToAction(nameof(Index));
            }
            string userNameId = returnUserNameId();
            ComplaintsVM complaintsVM2 = new ComplaintsVM()
            {
                complaints = new Complaints(),
                OrdersList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId && a.OrderStatus == SD.OrderStatusDone).
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
                    }),
                TicketResolutionList = SD.TicketResolution
            };
            return View(complaintsVM2);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateComplaint(ComplaintsVM complaintsVM)
        {
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {
                if (complaintsVM.complaints.Id != 0)
                {
                    if (complaintsVM.GeneralNotOrderRelated)
                    {
                        complaintsVM.complaints.OrderId = 0;
                        complaintsVM.complaints.TicketResolution = SD.NotRelevant;
                    }
                    _unitOfWork.Complaints.update(complaintsVM.complaints);
                }
                _unitOfWork.Save();
                //  ViewBag.storeExist = storeExist;
                ViewBag.success = true;
            }
            string uNameId = returnUserNameId();
            ComplaintsVM complaintsVM2 = new ComplaintsVM()
            {
                complaints = new Complaints(),
                OrdersList = _unitOfWork.Order.GetAll().
                Where(a => a.UserNameId == uNameId).Where(a => a.OrderStatus == SD.OrderStatusDone).
                Select(i => new SelectListItem
                {
                    Text = i.CustName + "- Id: " + i.Id,
                    Value = i.Id.ToString()
                }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.UserNameId == uNameId).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    }),
                TicketResolutionList = SD.TicketResolution
            };
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
