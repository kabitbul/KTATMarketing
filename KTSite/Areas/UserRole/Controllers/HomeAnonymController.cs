using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using KTSite.Models;

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
        public string authenticate(string authToken)
        {
            UsersForAPI usersForAPI = _unitOfWork.UsersForAPI.GetAll().Where(a => a.AuthToken == authToken).FirstOrDefault();
            if (usersForAPI == null)
                return "";
            return usersForAPI.UserId ;
        }
        [HttpGet("KTATAPI/TestAPI")]
        // [Authorize]
        public IActionResult TestAPI(string authToken)
        {
            string userId = authenticate(authToken);
            if (userId.Length > 0)
                return Ok("Test API is succesfull for KT Online Marketing");
            return Unauthorized();


        }
    }
}
