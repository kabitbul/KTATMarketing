using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KTSite.Models;
using Newtonsoft.Json;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization;

namespace KTSite.Areas.UserRole.Controllers
{
    [Area("UserRole")]
    [AllowAnonymous]
    [Authorize(Roles = SD.Role_Users)]
    public class HomeAnonymController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeAnonymController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            
                return View();
            }
    }
}
