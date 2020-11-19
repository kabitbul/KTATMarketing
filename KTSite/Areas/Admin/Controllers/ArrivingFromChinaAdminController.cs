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

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ArrivingFromChinaAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ArrivingFromChinaAdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {

            var arrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll();
            ViewBag.getProductName =  new Func<int, string>(getProductName);
            return View(arrivingFromChina);
        }
        public string getProductName(int ProductId)
        {
            return _unitOfWork.Product.GetAll().Where(a => a.Id == ProductId).Select(a => a.ProductName).FirstOrDefault();
        }
        [HttpPost]
        public IActionResult ApproveStatus(int[] Ids)
        {
            foreach(int Id in Ids)
            {
                ArrivingFromChina arrivingFromChina = _unitOfWork.ArrivingFromChina.GetAll().Where(a => a.Id == Id).FirstOrDefault();
                arrivingFromChina.UpdatedByAdmin = true;
                _unitOfWork.Save();
            }
            return View();
        }
        #region API CALLS

        #endregion
    }
}
