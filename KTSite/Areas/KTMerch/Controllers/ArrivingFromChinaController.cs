using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.KTMerch.Controllers
{
    [Area("KTMerch")]
    [Authorize(Roles = SD.Role_KTMerch)]
    public class ArrivingFromChinaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ArrivingFromChinaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            string uNameId = returnUserNameId();
            var arrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll().Where(a =>a.MerchId == uNameId);

            ViewBag.getProductName =  new Func<int, string>(getProductName);
            return View(arrivingFromChina);
        }
        public string getProductName(int ProductId)
        {
            return _unitOfWork.Product.GetAll().Where(a => a.Id == ProductId).Select(a => a.ProductName).FirstOrDefault();
        }
        [HttpPost]
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        #region API CALLS

        #endregion
    }
}
