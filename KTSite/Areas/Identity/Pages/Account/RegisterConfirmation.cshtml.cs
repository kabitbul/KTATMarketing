using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using EASendMail;

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
            var user2 = await _userManager.FindByEmailAsync(Email);
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
            var body = $"Please Confirm email Here  <a href={callbackUrl}>Confirm email Here!</a>";

            SmtpMail oMail = new SmtpMail("ES-E1646458156-01299-DUE5E8D722BE8A43-4713144F8EV7976D");
            // my Mail
            oMail.From = new MailAddress("KT Marketing", "litaltabibi@yahoo.com");
            //oMail.ReplyTo = "ktonlinemarketing1@gmail.com";
            // Set recipient email address
            oMail.To = Email;
            oMail.Subject = "Email Confirmation";
            oMail.HtmlBody = body;
            // Yahoo SMTP server address
            SmtpServer oServer = new SmtpServer("smtp.mail.yahoo.com");
            oServer.User =  "litaltabibi@yahoo.com";
            oServer.Password = "rzvsyleclxtbqckx";
            oServer.Port = 587;
            oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
            SmtpClient oSmtp = new SmtpClient();
            oSmtp.SendMail(oServer, oMail);

            //var senderEmail = new MailAddress("ktatmarketing1@gmail.com", "KT");
            //var receiverEmail = new MailAddress(Email, "Receiver");
            //var password = "sendmailsmail";
            
            //email confirmation end
            return Page();
        }
    }
}