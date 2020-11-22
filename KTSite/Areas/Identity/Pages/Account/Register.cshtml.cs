using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using KTSite.DataAccess.Repository.IRepository;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace KTSite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Name { get; set; }
            public string Role { get; set; }

            public IEnumerable<SelectListItem> RoleList { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public string returnUserNameId()
        {
            return (_unitOfWork.ApplicationUser.GetAll().Where(q => q.UserName == User.Identity.Name).Select(q => q.Id)).FirstOrDefault();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Name = Input.Name,
                    Email = Input.Email,
                    Role = Input.Role,
                    EmailConfirmed = true
                };
                 var result = _userManager.CreateAsync(user, Input.Password).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");


                    if (user.Role == null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Users);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)//user is signing in and not by admin
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            if(Input.Role == SD.Role_Users)
                            {
                                //System.Threading.Thread.Sleep(1000);
                                string userNameId =
                                _unitOfWork.ApplicationUser.GetAll().Where(a => a.Email == Input.Email && a.Name == Input.Name
                                && a.Role == SD.Role_Users).Select(a=> a.Id).FirstOrDefault();
                                PaymentBalance paymentBalance = new PaymentBalance();
                                paymentBalance.UserNameId = userNameId;
                                paymentBalance.Balance = 0;
                                paymentBalance.IsWarehouseBalance = false;
                                paymentBalance.AllowNegativeBalance = false;
                                _unitOfWork.PaymentBalance.Add(paymentBalance);
                                _unitOfWork.Save();
                    
                            
                            }
                            else if(Input.Role == SD.Role_Warehouse)
                            {
                                bool exist = _unitOfWork.PaymentBalance.GetAll().Any(a => a.IsWarehouseBalance);
                                if(!exist)
                                {
                                    //System.Threading.Thread.Sleep(1000);
                                    string userNameId =
                                _unitOfWork.ApplicationUser.GetAll().Where(a => a.Email == Input.Email && a.Name == Input.Name
                                && a.Role == SD.Role_Warehouse).Select(a => a.Id).FirstOrDefault();
                                    PaymentBalance paymentBalance = new PaymentBalance();
                                    paymentBalance.UserNameId = userNameId;
                                    paymentBalance.Balance = 0;
                                    paymentBalance.IsWarehouseBalance = true;
                                    paymentBalance.AllowNegativeBalance = true;
                                    _unitOfWork.PaymentBalance.Add(paymentBalance);
                                    _unitOfWork.Save();
                                }
                                
                            }
                            //admin is registering a new user
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
