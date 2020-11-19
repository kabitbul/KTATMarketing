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
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class InventoryAnalysisController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryAnalysisController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string? Id)
        {
            int avgDay;
            int timeToShip;
            if (Id == null)
            {
                avgDay = 3;
                timeToShip = 100;
            }
            else
            {
                string[] str = Id.Split('_');
                avgDay = Convert.ToInt32(str[0]);
                timeToShip = Convert.ToInt32(str[1]);

            }
            List<InventoryAnalysis> InventoryNeedRestock = new List<InventoryAnalysis>();
            ViewBag.getProductName = new Func<int, string>(returnProductName);
            var product = _unitOfWork.Product.GetAll().Where(a => a.ReStock);
            foreach(Product prod in product)
            {
                if (prod.InventoryCount <= 0)
                {
                    continue;
                }
                double NumOfAvgSell = 0;
                int Count = _unitOfWork.Order.GetAll().Where(a => a.ProductId == prod.Id &&
                              a.OrderStatus != SD.OrderStatusCancelled &&
                              a.UsDate.Date >= DateTime.Now.AddDays((avgDay*(-1))).Date &&
                              a.UsDate.Date <= DateTime.Now.AddDays(-1).Date).Sum(a => a.Quantity);
                if(Count > 0)
                {
                    NumOfAvgSell = (double)Count / avgDay;
                }
                
                if((prod.MadeIn == SD.MadeInChina &&(NumOfAvgSell * timeToShip) > (prod.InventoryCount + prod.OnTheWayInventory))||
                    (prod.MadeIn == SD.MadeInUSA && (NumOfAvgSell * 20) > prod.InventoryCount + prod.OnTheWayInventory))
                {
                    InventoryAnalysis InvObj = new InventoryAnalysis();
                    InvObj.ProductId = prod.Id;
                    if (prod.MadeIn == SD.MadeInChina)
                    {
                        InvObj.MissingQuantity = (NumOfAvgSell * timeToShip) - (prod.InventoryCount+prod.OnTheWayInventory);
                    }
                    else
                    {
                        InvObj.MissingQuantity = (NumOfAvgSell * 20) - (prod.InventoryCount + prod.OnTheWayInventory);
                    }
                    InvObj.AvgSales = NumOfAvgSell;
                    InvObj.Cost = prod.Cost;
                    InvObj.InventoryCount = prod.InventoryCount;
                    InvObj.OnTheWay = prod.OnTheWayInventory;
                    InvObj.OwnByWarehouse = prod.OwnByWarehouse;
                    InvObj.AvgDays = avgDay;
                    InvObj.TimeToArrive = timeToShip;
                    InventoryNeedRestock.Add(InvObj);
                }
            }
            return View(InventoryNeedRestock);
        }
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }
        public IActionResult CheckInventory()
        {
            InventoryAnalysis inventoryAnalysis = new InventoryAnalysis();
            //ViewBag.getStore =
            //   new Func<string, string>(getStore);
            return View(inventoryAnalysis);
        }
        public string getUserName(string unameId)
        {
            return _unitOfWork.ApplicationUser.GetAll().Where(a => a.Id == unameId).Select(a => a.Name).FirstOrDefault();
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        [HttpPost]
        public IActionResult CheckInventory(string id)
        {
            if (ModelState.IsValid)
            {
             //   return RedirectToAction(nameof(Index), 
             //       new { id = inventoryAnalysis.AvgDays.ToString()+"_"+ inventoryAnalysis.TimeToArrive.ToString() });
            }
            return RedirectToAction(nameof(Index), new { id = "2" });
            // return RedirectToAction(nameof(Index));
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
