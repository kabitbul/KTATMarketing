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

namespace KTSite.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Authorize(Roles = SD.Role_Warehouse)]
    public class ReturningItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReturningItemController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var returningItemList = _unitOfWork.ReturningItem.GetAll().OrderByDescending(q=>q.Id);
            ViewBag.getProductName =
               new Func<int, string>(returnProductName);
            ViewBag.errSaveInProgress = false;
            return View(returningItemList);
        }
        public IActionResult AddReturningItem()
        {
            ReturningItemVM returningItemVM = new ReturningItemVM()
            {
                returningItems = new ReturningItem(),
                ReturningItemStatusList = SD.ReturningItemStatus,
                ProductList = _unitOfWork.Product.GetAll().Select(i => new SelectListItem
                {
                    Text = i.ProductName,
                    Value = i.Id.ToString()
                })
            };
            
            ViewBag.sysDate = DateTime.Now;
            ViewBag.ShowMsg = 0;
            ViewBag.failed = false;
            return View(returningItemVM);
        }
        
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReturningItem(ReturningItemVM returningItemVM)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ShowMsg = 1;
                _unitOfWork.ReturningItem.Add(returningItemVM.returningItems);
                Product product = _unitOfWork.Product.GetAll().Where(a => a.Id == returningItemVM.returningItems.ProductId).FirstOrDefault();
                if (returningItemVM.returningItems.ItemStatus == SD.ReturningItemAdd)
                {
                    product.InventoryCount = product.InventoryCount + returningItemVM.returningItems.Quantity;
                }
                else if (returningItemVM.returningItems.ItemStatus == SD.ReturningItemRemove)
                {
                    product.InventoryCount = product.InventoryCount - returningItemVM.returningItems.Quantity;
                }
                _unitOfWork.Save();
            }
                ReturningItemVM returningItemVM2 = new ReturningItemVM()
                {
                    returningItems = new ReturningItem(),
                    ReturningItemStatusList = SD.ReturningItemStatus,
                    ProductList = _unitOfWork.Product.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.ProductName,
                        Value = i.Id.ToString()
                    })
                };
            return View(returningItemVM2);
        }
        public int getProductIdByName(string productName)
        {
            return _unitOfWork.Product.GetAll().Where(a => a.ProductName.Equals(productName,StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id).FirstOrDefault();
        }
        public void initializeVM(OrderVM orderVM)
        {
            orderVM.Orders.Id = 0;
            orderVM.Orders.ProductId = 0;
            orderVM.Orders.CustStreet2= "";
            orderVM.Orders.CustPhone= "";
            orderVM.Orders.IsAdmin = true;
            orderVM.Orders.OrderStatus = SD.OrderStatusAccepted;
        }
        public void updateInventory(int productId, int quantity)
        {
            Product product =_unitOfWork.Product.GetAll().Where(a => a.Id == productId).FirstOrDefault();
            product.InventoryCount = product.InventoryCount - quantity;
        }
        public void updateWarehouseBalance(int quantity)
        {
            PaymentBalance paymentBalance = _unitOfWork.PaymentBalance.GetAll().Where(a => a.IsWarehouseBalance).FirstOrDefault();
            paymentBalance.Balance = paymentBalance.Balance - (quantity * SD.shipping_cost);
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
