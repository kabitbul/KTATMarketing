﻿using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using EASendMail;

namespace KTSite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                //var senderEmail = new MailAddress("ktatmarketing1@gmail.com", "KT");
                //var receiverEmail = new MailAddress(Input.Email, "Receiver");
                //var password = "sendmailsmail";
                //var sub = "Reset Password";
                //var user = await _userManager.FindByEmailAsync(Input.Email);
                //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return RedirectToPage("./ForgotPasswordConfirmation");
                //}
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                //var callbackUrl = Url.Page(
                //      "/Account/ResetPassword",
                //        pageHandler: null,
                //        values: new { area = "Identity", code },
                //        protocol: Request.Scheme);
                //var body = $"Please reset your password Here {HtmlEncoder.Default.Encode(callbackUrl)}.";
                //var smtp = new SmtpClient
                //{
                //    Host = "smtp.gmail.com",
                //    Port = 587,
                //    EnableSsl = true,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    UseDefaultCredentials = false,
                //    Credentials = new NetworkCredential(senderEmail.Address, password)
                //};

                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new NetworkCredential(senderEmail.Address, password);
                //smtp.EnableSsl = true;
                //using (var mess = new MailMessage(senderEmail, receiverEmail)
                //{
                //    Subject = "Reset Password",
                //    Body = body
                //})
                //{
                //    smtp.Send(mess);
                //    return RedirectToPage("./ForgotPasswordConfirmation");

                //}

                //////////////////////////////////////////////////////////////////////
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                      "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { area = "Identity", code },
                        protocol: Request.Scheme);
                var body = $"Please reset password Here <a href={callbackUrl}>Reset Password Here!</a>";
                SmtpMail oMail = new SmtpMail("ES-E1646458156-01299-DUE5E8D722BE8A43-4713144F8EV7976D");
                // my Mail
                oMail.From = new MailAddress("KT Marketing", "litaltabibi@yahoo.com");
                //oMail.ReplyTo = "ktonlinemarketing1@gmail.com";
                // Set recipient email address
                oMail.To = Input.Email;
                oMail.Subject = "Reset Password";
                oMail.HtmlBody = body;
                // Yahoo SMTP server address
                SmtpServer oServer = new SmtpServer("smtp.mail.yahoo.com");
                oServer.User = "litaltabibi@yahoo.com";
                oServer.Password = "rzvsyleclxtbqckx";
                oServer.Port = 587;
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.SendMail(oServer, oMail);
                return RedirectToPage("./ForgotPasswordConfirmation");
            }
                return Page();
            
        }
    }
}
