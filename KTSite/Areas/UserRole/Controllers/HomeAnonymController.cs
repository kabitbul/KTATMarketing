﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            //var phone = new DefaultDisplayMode("Phone")
            //{
            //    ContextCondition = ctx => ctx.GetOverriddenUserAgent() != null && ctx.GetOverriddenUserAgent().Contains("iPhone")
            //};
                return View();
            }
    }
}
