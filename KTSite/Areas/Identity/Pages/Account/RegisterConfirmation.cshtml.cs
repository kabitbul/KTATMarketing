using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Net;

namespace KTSite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            //DisplayConfirmAccountLink = true;
            //if (DisplayConfirmAccountLink)
            //{
            //    var userId = await _userManager.GetUserIdAsync(user);
            //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //    EmailConfirmationUrl = Url.Page(
            //        "/Account/ConfirmEmail",
            //        pageHandler: null,
            //        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
            //        protocol: Request.Scheme);
            //}
            //email confirmation start
            var senderEmail = new MailAddress("ktatmarketing1@gmail.com", "KT");
            var receiverEmail = new MailAddress(Email, "Receiver");
            var password = "sendmailsmail";
            var sub = "Email Confirmation";
            var user2 = await _userManager.FindByEmailAsync(Email);
            if (user2 == null ||(await _userManager.IsEmailConfirmedAsync(user2)))
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
            return Page();
        }
    }
}
