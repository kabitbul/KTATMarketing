﻿using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ComplaintsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ComplaintsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var complaints = _unitOfWork.Complaints.GetAll().Where(a=>a.IsAdmin && (a.MerchType == null || a.MerchType == ""));
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
            var complaints = _unitOfWork.Complaints.GetAll().Where(a =>!a.IsAdmin && (a.MerchType == null || a.MerchType == ""));
            ViewBag.Refunded = new Func<string, bool>(returnIsRefunded);
            return View(complaints);
        }
        public IActionResult ShowMerchComplaint()
        {
            ViewBag.getUserName =
                   new Func<string, string>(getUserName);
            var complaints = _unitOfWork.Complaints.GetAll().Where(a => a.MerchType != null && a.MerchType != "");
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
            if (Id != null)
            {
                complaintsVM = new ComplaintsVM()
                {
                    complaints = new Complaints(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.Id == Id).
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
                }),
                    TicketResolutionList = SD.TicketResolution
                };
                Order ord = _unitOfWork.Order.Get((long)Id);
                
            }
            else
            {
                complaintsVM = new ComplaintsVM()
                {
                    complaints = new Complaints(),
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
                }),
                    TicketResolutionList = SD.TicketResolution
                };
            }
            
            if (Id != null)
            {
                complaintsVM.complaints.OrderId = Id;
            }

            ViewBag.UNameId = uNameId;
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = false;
            ViewBag.failed = false;
            ViewBag.success = true;
            return View(complaintsVM);
        }
        public IActionResult UpdateComplaint(long Id)
        {
            Complaints comp = _unitOfWork.Complaints.GetAll().Where(a => a.Id == Id).FirstOrDefault();
            bool IsAdmin = comp.IsAdmin;
            if(comp.MerchType == null || comp.MerchType == "")
            {
                ViewBag.isMerch = false;
            }
            else
            {
                ViewBag.isMerch = true;
            }
            
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
                    }),
                TicketResolutionList = SD.TicketResolution
            };
            ViewBag.IsAdmin = IsAdmin;
            if(complaintsVM.complaints.OrderId == 0) 
            {
                complaintsVM.GeneralNotOrderRelated = true;
            }
            else
            {
                complaintsVM.GeneralNotOrderRelated = false;
            }
            ViewBag.success = true;
            ViewBag.ShowMsg = false;
            return View(complaintsVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComplaint(ComplaintsVM complaintsVM)
        {
            ViewBag.ShowMsg = true;
            ViewBag.success = false;
            if (ModelState.IsValid)
            {
                complaintsVM.complaints.IsAdmin = true;
                complaintsVM.complaints.UserNameId = returnUserNameId();
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
                        complaintsVM.complaints.MerchType = ord.MerchType;
                        complaintsVM.complaints.MerchId = ord.MerchId;
                       //added charge of warehouse on 18/03/2023 
                       if(complaintsVM.complaints.TicketResolution == SD.Refund)
                       {
                        double shipCharge = 
                       _unitOfWork.Product.GetAll().Where(a=>a.Id == ord.ProductId).
                       Select(a=> a.ShippingCharge).FirstOrDefault();
                       shipCharge = shipCharge * ord.Quantity;
                        PaymentBalance pb = _unitOfWork.PaymentBalance.GetAll()
                          .Where(a=> a.IsWarehouseBalance).FirstOrDefault();
                       pb.Balance = pb.Balance + shipCharge;
                        Refund refund= new Refund();
                          refund.OrderId = ord.Id;
                         /******/ refund.RefundedBy = 
                     _unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name).FirstOrDefault();
                       _unitOfWork.Save();
                        refund.RefundDate = DateTime.Now;
                        refund.RefundQuantity = ord.Quantity;
                        refund.Cost = shipCharge;
                        refund.Quantity = ord.Quantity;
                        refund.StoreNameId = ord.StoreNameId;
                        refund.UserNameId = ord.UserNameId;
                        refund.AmountRefunded = shipCharge;
                        _unitOfWork.Refund.Add(refund);
                      }//END added charge of warehouse on 18/03/2023
                    }
                    _unitOfWork.Complaints.Add(complaintsVM.complaints);
                   
                }
                else
                {
                    if (complaintsVM.GeneralNotOrderRelated)
                    {
                        complaintsVM.complaints.OrderId = 0;
                        complaintsVM.complaints.StoreId = 0;
                        complaintsVM.complaints.TicketResolution = SD.NotRelevant;
                    }
                    else
                    {
                        Order ord = _unitOfWork.Order.Get((long)complaintsVM.complaints.OrderId);
                        complaintsVM.complaints.ProductName = ord.ProductName;
                        complaintsVM.complaints.CustName = ord.CustName;
                        complaintsVM.complaints.MerchType = ord.MerchType;
                        complaintsVM.complaints.MerchId = ord.MerchId;

                    }
                    _unitOfWork.Complaints.update(complaintsVM.complaints);
                }

                _unitOfWork.Save();
                ViewBag.success = true;


                //return RedirectToAction(nameof(Index));
            }
            string userNameId = returnUserNameId();
            ComplaintsVM complaintsVM2 = new ComplaintsVM()
            {
                complaints = new Complaints(),
                OrdersList = _unitOfWork.Order.GetAll().Where(a => a.UserNameId == userNameId && a.OrderStatus == SD.OrderStatusDone
                && a.Id == complaintsVM.complaints.OrderId).
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
                if (complaintsVM.complaints.MerchType == null || complaintsVM.complaints.MerchType == "")
                {
                    ViewBag.isMerch = false;
                }
                else
                {
                    ViewBag.isMerch = true;
                }
                complaintsVM.complaints.HandledBy =
                    (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Name)).FirstOrDefault();
            if(complaintsVM.GeneralNotOrderRelated)
                {
                    complaintsVM.complaints.OrderId = 0;
                    complaintsVM.complaints.TicketResolution = SD.NotRelevant;
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
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => a.IsAdmin && a.OrderStatus == SD.OrderStatusDone ).
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
                    }),
                    TicketResolutionList = SD.TicketResolution
                };
            }
            else
            {
                complaintsVM2 = new ComplaintsVM()
                {
                    complaints = _unitOfWork.Complaints.GetAll().Where(a => a.Id == complaintsVM.complaints.Id).FirstOrDefault(),
                    OrdersList = _unitOfWork.Order.GetAll().Where(a => !a.IsAdmin && a.OrderStatus == SD.OrderStatusDone).
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
                    }),
                    TicketResolutionList = SD.TicketResolution
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
