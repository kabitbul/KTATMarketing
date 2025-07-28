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
using Microsoft.AspNetCore.Mvc.Rendering;
using MailKit.Search;
using System.IO;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmazonStoresController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AAmazonStoresController(IUnitOfWork unitOfWork , IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
          List<AAmazonStores> lst = _unitOfWork.AAmazonStores.GetList();
          
            return View(lst);
        }
   public IActionResult Upsert(int id)
        {
            ViewBag.ShowMsg = false;
            ViewBag.failed = false;
            AAmazonStores aAmazonStores = new AAmazonStores();
            if(id == null || id == 0)//create
            {
                return View(aAmazonStores);
            }
            aAmazonStores = _unitOfWork.AAmazonStores.GetById(id);
            if (aAmazonStores == null)
            {
                return NotFound();
            }
            return View(aAmazonStores);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(AAmazonStores aAmazonStores)
        {
            ViewBag.ShowMsg = true;
            if (ModelState.IsValid)
            {
                if(aAmazonStores.Id == 0)
                {
                    _unitOfWork.AAmazonStores.InsertStore(aAmazonStores.StoreName);
                }
                else
                {
                    _unitOfWork.AAmazonStores.updateById(aAmazonStores.Id,aAmazonStores.StoreName);
                }
                _unitOfWork.Save();
                ViewBag.failed = false;
                return RedirectToAction(nameof(Index));
            }
            ViewBag.failed = true;
            return View(aAmazonStores);
        }
    }
}
