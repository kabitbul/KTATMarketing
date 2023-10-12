using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Collections.Generic;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class InventoryOnTexasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryOnTexasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<InventoryOnTexasSumList> invList = 
                  _unitOfWork.inventoryOnTexas.GetInventoryOnTexasSum();
            return View(invList);
        }
    }
}
