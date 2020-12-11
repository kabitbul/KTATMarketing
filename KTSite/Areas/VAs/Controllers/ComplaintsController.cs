using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.VAs.Controllers
{
    [Area("VAs")]
    [Authorize(Roles = SD.Role_VAs)]
    public class ComplaintsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComplaintsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var complaints = _unitOfWork.Complaints.GetAll().Where(a=>a.IsAdmin);
            ViewBag.getStore =
               new Func<string, string>(getStore);
            ViewBag.IsAdmin = new Func<string, bool>(returnIsAdmin);
            return View(complaints);
        }
        public bool returnIsAdmin(string OrderId)
        {
            return _unitOfWork.Order.GetAll().Where(a => a.Id == Convert.ToInt64(OrderId)).Select(a => a.IsAdmin).FirstOrDefault();
        }
        public IActionResult ShowUserComplaint()
        {
            ViewBag.getUserName =
                   new Func<string, string>(getUserName);
            var complaints = _unitOfWork.Complaints.GetAll().Where(a =>!a.IsAdmin);
            ViewBag.Refunded = new Func<string, bool>(returnIsRefunded);
            return View(complaints);
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
        public string getUserName(string unameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == unameId).Select(a => a.Name).FirstOrDefault();
        }
        public string getStore(string storeId)
        {
            return _unitOfWork.UserStoreName.GetAll().Where(a => a.Id == Convert.ToInt32(storeId)).Select(a => a.StoreName).FirstOrDefault();
        }
        public IActionResult AddComplaint(long? Id)
        {
            ComplaintsVM complaintsVM;
            string uNameId = "";
            string uName = "";

            uNameId = returnUserNameId();
            uName = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.UserName)).FirstOrDefault();
            //if (Id == null)
            //{
                complaintsVM = new ComplaintsVM()
                {
                    complaints = new Complaints(),
                    // OrdersList = _unitOfWork.Complaints.getAllOrdersForAdmin().
                    //Select(i => new SelectListItem
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.OrderStatus == SD.OrderStatusDone &&
                                     !_unitOfWork.Complaints.GetAll().Any(p => p.OrderId == a.Id)).
                      Select(i => new SelectListItem
                      {
                        Text = i.CustName + "- Id: " + i.Id,
                        Value = i.Id.ToString()
                    }),
                    StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
                }; 
            if (Id != null)
            {
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
            bool IsAdmin = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).Select(a => a.IsAdmin).FirstOrDefault();
            Complaints comp = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).FirstOrDefault();
            string uNameId = comp.UserNameId;
            ComplaintsVM complaintsVM;
            complaintsVM = new ComplaintsVM()
            {
                complaints = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).FirstOrDefault(),
                OrdersList = _unitOfWork.Order.GetAll().Where(a => a.Id == comp.OrderId).
                     Select(i => new SelectListItem
                     {
                         Text = i.CustName + "- Id: " + i.Id,
                         Value = i.Id.ToString()
                     }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
            };
            ViewBag.IsAdmin = IsAdmin;
            if (complaintsVM.complaints.OrderId == 0)
            {
                complaintsVM.GeneralNotOrderRelated = true;
            }
            else
            {
                complaintsVM.GeneralNotOrderRelated = false;
            }
            ViewBag.ShowMsg = false;
            ViewBag.success = true;
            return View(complaintsVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComplaint(ComplaintsVM complaintsVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            string userNameId = returnUserNameId();
            if (ModelState.IsValid)
            {
                //complaintsVM.complaints.IsAdmin = _unitOfWork.Order.GetAll().Where(a => a.Id == complaintsVM.complaints.OrderId).
                //    Select(a => a.IsAdmin).FirstOrDefault();
                complaintsVM.complaints.IsAdmin = true;
                complaintsVM.complaints.UserNameId = userNameId;
                if (complaintsVM.complaints.Id == 0)
                {
                    if (complaintsVM.GeneralNotOrderRelated)
                    {
                        complaintsVM.complaints.OrderId = 0;
                    }
                    else//if its not a general ticket, get the storeid based on order
                    {
                        Order ord = _unitOfWork.Order.Get((long)complaintsVM.complaints.OrderId);
                        complaintsVM.complaints.StoreId = ord.StoreNameId;
                        complaintsVM.complaints.ProductName = ord.ProductName;
                        complaintsVM.complaints.CustName = ord.CustName;
                    }
                    _unitOfWork.Complaints.Add(complaintsVM.complaints);
                }
                else
                {
                    if (complaintsVM.GeneralNotOrderRelated)
                    {
                        complaintsVM.complaints.OrderId = 0;
                        //complaintsVM.complaints.StoreId = 0;
                    }
                    _unitOfWork.Complaints.update(complaintsVM.complaints);
                }
                _unitOfWork.Save();
                ViewBag.success = true;


                //return RedirectToAction(nameof(Index));
            }
            ComplaintsVM complaintsVM2 = new ComplaintsVM()
            {
                complaints = new Complaints(),
                OrdersList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId).Where(a => a.OrderStatus == SD.OrderStatusDone).
    Select(i => new SelectListItem
    {
        Text = i.CustName + "- Id: " + i.Id,
        Value = i.Id.ToString()
    }),
                StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
            };
            return View(complaintsVM2);
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
                if (complaintsVM.GeneralNotOrderRelated)
                {
                    complaintsVM.complaints.OrderId = 0;
                }
                if (!complaintsVM.GeneralNotOrderRelated && complaintsVM.complaints.Solved &&
                    complaintsVM.complaints.NewTrackingNumber == null && complaintsVM.complaints.SolutionDesc == null)
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
            ComplaintsVM complaintsVM2;
            if (complaintsVM.complaints.IsAdmin)
            {
                complaintsVM2 = new ComplaintsVM()
                {
                    complaints = _unitOfWork.Complaints.GetAll().Where(a => a.Id == complaintsVM.complaints.Id).FirstOrDefault(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.IsAdmin).Where(a => a.OrderStatus == SD.OrderStatusDone).
                    Select(i => new SelectListItem
                    {
                        Text = i.CustName + "- Id: " + i.Id,
                        Value = i.Id.ToString()
                    }),
                    StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
                };
            }
            else
            {
                complaintsVM2 = new ComplaintsVM()
                {
                    complaints = _unitOfWork.Complaints.GetAll().Where(a => a.Id == complaintsVM.complaints.Id).FirstOrDefault(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => !a.IsAdmin).Where(a => a.OrderStatus == SD.OrderStatusDone).
                    Select(i => new SelectListItem
                    {
                        Text = i.CustName + "- Id: " + i.Id,
                        Value = i.Id.ToString()
                    }),
                    StoresList = _unitOfWork.UserStoreName.GetAll().Where(a => a.IsAdminStore).
                    Select(i => new SelectListItem
                    {
                        Text = i.StoreName,
                        Value = i.Id.ToString()
                    })
                };
            }
            ViewBag.IsAdmin = complaintsVM2.complaints.IsAdmin;
            return View(complaintsVM2);
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
            var complaints2 = _unitOfWork.Complaints.GetAll().Where(a => a.IsAdmin);
            ViewBag.getStore =
               new Func<string, string>(getStore);
            ViewBag.IsAdmin = new Func<string, bool>(returnIsAdmin);
            return View(complaints2);
            //return View();
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
