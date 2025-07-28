using System;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KTSite.Utility;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmzInventoryKTUSController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        int gVarStoreId = SD.KTStoreId;//---------GLOBAL CONST
        string gVarMarketplace = SD.marketPlaceUS;//--------GLOBAL CONST
        public AAmzInventoryKTUSController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(bool? showRestock )
        {
           List<AmazonInvStatistics> invList = 
           _unitOfWork.AAmzFBAInventory.inventoryIndexData(showRestock,gVarMarketplace,gVarStoreId);
           return View(invList);
        }
        [HttpPost]
        public IActionResult UpdateRestockND(int id, bool restockNotDecided)
          {
           string sql = "";
           if(id > 0)
            {
              if(gVarMarketplace == SD.marketPlaceUS)
                { 
                    sql = " UPDATE AAmzAsinToSku " +
  "                         SET RestockNOTDECIDED = "+(restockNotDecided ? 1:0)+" " +
  "                         WHERE Id = " +id+";";
                 }
              else if(gVarMarketplace == SD.marketPlaceCA)
               {
               sql = " UPDATE AAmzAsinToSku " +
  "                    SET RestockNOTDECIDEDCA = "+(restockNotDecided ? 1:0)+" " +
  "                    WHERE Id = " +id+";"; 
                }
              int res = _unitOfWork.AAmzAsinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }
       [HttpPost]
        public IActionResult UpdateRestock(int id, bool restock)
        {
            string sql = "";
           if(id > 0)
            {
             if(gVarMarketplace == SD.marketPlaceUS)
              { 
              sql = " UPDATE AAmzAsinToSku " +
  "                   SET RestockUS = "+(restock ? 1:0)+" " +
  "                   WHERE Id = " +id+";";
              }
              else if(gVarMarketplace == SD.marketPlaceCA)
               {
                sql = " UPDATE AAmzAsinToSku " +
  "                     SET RestockCA = "+(restock ? 1:0)+" " +
  "                     WHERE Id = " +id+";"; 

                }
              int res = _unitOfWork.AAmzAsinToSku.updateRestockStatus(sql);
            }
                       
            return View();
        }
        #region API CALLS
        #endregion
 } 
}
