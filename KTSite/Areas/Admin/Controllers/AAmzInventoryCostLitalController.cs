using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmzInventoryCostLitalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        int gVarStoreId = SD.LitalStoreId;//---------GLOBAL CONST
        
        public AAmzInventoryCostLitalController(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           var vm = new AAmzInventoryCostVM
    {
        LatestItems = _unitOfWork.AAmzInventoryCost.GetByLastDate(gVarStoreId),
        USHistory = _unitOfWork.AAmzInventoryCost.GetList(gVarStoreId, "US", 60),
        CAHistory = _unitOfWork.AAmzInventoryCost.GetList(gVarStoreId, "CA", 60)
    };

    return View(vm);
        }
   
    }
}


