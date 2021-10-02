using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class ReturningItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReturningItemController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string uNameId = (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).
                Select(q => q.Id)).FirstOrDefault();

            var returningItemList = _unitOfWork.ReturningItem.GetAll().Join(_unitOfWork.Product.GetAll().Where(a=> a.MerchId == uNameId),
                                                        returningItemList => returningItemList.ProductId,
                                                        product => product.Id,
                                                        (returningItemList, product) => new
                                                        {
                                                            returningItemList
                                                        }).Select(a => a.returningItemList);
                
            
            ViewBag.getProductName =
               new Func<int, string>(returnProductName);
            ViewBag.errSaveInProgress = false;
            return View(returningItemList);
        }
        
        
        public string returnProductName(int productId)
        {
            return (_unitOfWork.Product.GetAll().Where(q => q.Id == productId).Select(q => q.ProductName)).FirstOrDefault();
        }

        public int getProductIdByName(string productName)
        {
            return _unitOfWork.Product.GetAll().Where(a => a.ProductName.Equals(productName,StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id).FirstOrDefault();
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
