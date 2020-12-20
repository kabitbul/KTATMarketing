using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.VAs.Controllers
{
    [Area("VAs")]
    [Authorize(Roles = SD.Role_VAs)]
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
            ViewBag.success = true;
            ViewBag.ShowMsg = false;
            ViewBag.userName = (_unitOfWork.ApplicationUser.GetAll().
                Where(q => q.UserName == User.Identity.Name).
                Select(q => q.Name)).FirstOrDefault();
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
            ViewBag.success = false;
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {
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
                ViewBag.success = true;
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

        #endregion
    }
}
