using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var provider = AuthenticationConstants.WinthropScheme;
            
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", "Callback", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
    }
}
