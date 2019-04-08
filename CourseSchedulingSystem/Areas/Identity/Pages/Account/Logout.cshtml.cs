using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var redirectUrl = Url.Page("./Login", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(AuthenticationConstants.WinthropScheme, redirectUrl);
            return SignOut(
                properties,
                IdentityConstants.ApplicationScheme,
                IdentityConstants.ExternalScheme,
                IdentityConstants.TwoFactorUserIdScheme,
                AuthenticationConstants.WinthropScheme
            );
        }
    }
}