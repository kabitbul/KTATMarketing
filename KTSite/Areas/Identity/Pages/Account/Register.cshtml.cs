using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
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
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string PhoneNumber { get; set; }
            public string Role { get; set; }

            public IEnumerable<SelectListItem> RoleList { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Where(x=> x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
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
            bool existErr = false;
            if (ModelState.IsValid)
            {
                //fields validations
                bool validPhone; 
                if (Input.PhoneNumber == null)
                {
                    validPhone = false;
                }
                else
                {
                    validPhone = Regex.IsMatch(Input.PhoneNumber, SD.MatchPhonePattern);
                }
                if(!validPhone && (Input.Role == null || Input.Role == "Users"))
                {
                    ModelState.AddModelError(string.Empty, "Phone Not Valid.");
                    existErr = true;
                }
                if(Input.StreetAddress == null && (Input.Role == null || Input.Role == "Users"))
                {
                    ModelState.AddModelError(string.Empty, "Street Can't be empty.");
                    existErr = true;
                }
                if (Input.City == null && (Input.Role == null || Input.Role == "Users"))
                {
                    ModelState.AddModelError(string.Empty, "City Can't be empty.");
                    existErr = true;
                }
                if (Input.State == null && (Input.Role == null || Input.Role == "Users"))
                {
                    ModelState.AddModelError(string.Empty, "State Can't be empty.");
                    existErr = true;
                }
                if (Input.PostalCode == null && (Input.Role == null || Input.Role == "Users"))
                {
                    ModelState.AddModelError(string.Empty, "Postal code Can't be empty.");
                    existErr = true;
                }
                if (existErr && (Input.Role == null || Input.Role == "Users"))
                {
                    Input = new InputModel()
                    {
                        RoleList = _roleManager.Roles.Where(x => x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
                        {
                            Text = i,
                            Value = i
                        })
                    };
                    return Page();
                }
                //fields validations
                var user = new ApplicationUser();
                if (Input.Role == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Name = Input.Name,
                        Email = Input.Email,
                        PhoneNumber = Input.PhoneNumber,
                        StreetAddress = Input.StreetAddress,
                        City = Input.City,
                        State = Input.State,
                        PostalCode = Input.PostalCode,
                        Role = "Users",
                        EmailConfirmed = false
                    };
                }
                else
                {
                    if (Input.Role == "Users")
                    {
                        user = new ApplicationUser
                        {
                            UserName = Input.Email,
                            Name = Input.Name,
                            Email = Input.Email,
                            StreetAddress = Input.StreetAddress,
                            City = Input.City,
                            State = Input.State,
                            PostalCode = Input.PostalCode,
                            Role = Input.Role,
                            EmailConfirmed = false
                        };
                    }
                    else
                    {
                        user = new ApplicationUser
                        {
                            UserName = Input.Email,
                            Name = Input.Name,
                            Email = Input.Email,
                            Role = Input.Role,
                            EmailConfirmed = true
                        };
                    }
                }    
                 var result = _userManager.CreateAsync(user, Input.Password).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");


                    if (user.Role == null)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_Users);
                        _userManager.Options.SignIn.RequireConfirmedEmail = true;
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }
                    if(Input.Role == null)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (user.Role == null)//user is signing in and not by admin
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            string userNameId =
                                _unitOfWork.ApplicationUser.GetAll().Where(a => a.Email == Input.Email && a.Name == Input.Name
                                ).Select(a => a.Id).FirstOrDefault();
                            PaymentBalance paymentBalance = new PaymentBalance();
                            paymentBalance.UserNameId = userNameId;
                            paymentBalance.Balance = 0;
                            paymentBalance.IsWarehouseBalance = false;
                            paymentBalance.AllowNegativeBalance = false;
                            _unitOfWork.PaymentBalance.Add(paymentBalance);
                            _unitOfWork.Save();
                            
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
                                //email confirmation start
                                var senderEmail = new MailAddress("ktatmarketing1@gmail.com", "KT");
                                var receiverEmail = new MailAddress(Input.Email, "Receiver");
                                var password = "sendmailsmail";
                                var sub = "Email Confirmation";
                                var user2 = await _userManager.FindByEmailAsync(Input.Email);
                                if (user2 == null || (await _userManager.IsEmailConfirmedAsync(user2)))
                                {
                                    // Don't reveal that the user does not exist or is not confirmed
                                    return RedirectToPage("./ForgotPasswordConfirmation");
                                }
                                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user2);
                                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                                var callbackUrl = Url.Page(
                                    "/Account/ConfirmEmail",
                                    pageHandler: null,
                                    values: new { userId = user.Id, code = code },
                                    protocol: Request.Scheme);
                                //var body = $"Please Confirm email Here {HtmlEncoder.Default.Encode(callbackUrl)}.";
                                var body = $"Please Confirm email Here {callbackUrl}.";
                                var smtp = new SmtpClient
                                {
                                    Host = "smtp.gmail.com",
                                    Port = 587,
                                    EnableSsl = true,
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = false,
                                    Credentials = new NetworkCredential(senderEmail.Address, password)
                                };

                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new NetworkCredential(senderEmail.Address, password);
                                smtp.EnableSsl = true;
                                using (var mess = new MailMessage(senderEmail, receiverEmail)
                                {
                                    Subject = "Email Confirmation",
                                    Body = body
                                })
                                {
                                    smtp.Send(mess);
                                    //return LocalRedirect(returnUrl);
                                    //return RedirectToPage("./Login");

                                }
                                //email confirmation end

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
                RoleList = _roleManager.Roles.Where(x => x.Name != "Admin").Select(x => x.Name).Select(i => new SelectListItem
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
