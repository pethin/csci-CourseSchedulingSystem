using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CourseSchedulingSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty] public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData] public string ErrorMessage { get; set; }

        [TempData] public string UserName { get; set; }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", "Callback", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            // Sign in the user with this external login provider if the user already has a login. If external login does not exist call OnExternalLoginDoesNotExist.
            return await LogInWithExternalProvider(info, false, true, returnUrl, OnExternalLoginDoesNotExist);
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(UserName);

                if (user == null)
                {
                    ErrorMessage = "Error finding user during confirmation.";
                    return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
                }

                if (await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                        return await LogInWithExternalProvider(info, false, true, returnUrl, OnExternalLoginFailed);

                    foreach (var error in addLoginResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }

                ErrorMessage = "Wrong password.";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }

        private async Task<IActionResult> LogInWithExternalProvider(ExternalLoginInfo info, bool isPersistent,
            bool bypassTwoFactor, string returnUrl, OnExternalLoginFail onFailure)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                isPersistent, bypassTwoFactor);

            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name,
                    info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut) return RedirectToPage("./Lockout");

            return await onFailure(info, returnUrl);
        }

        private async Task<IActionResult> OnExternalLoginDoesNotExist(ExternalLoginInfo info, string returnUrl)
        {
            ReturnUrl = returnUrl;
            LoginProvider = info.LoginProvider;

            UserName = GetUserNameFromInfo(info);

            if (UserName == null)
            {
                ErrorMessage = "Could not determine user name.";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
            }

            // If a user already exists with the user name.
            var existingUser = await _userManager.FindByNameAsync(UserName);
            if (existingUser != null)
            {
                // If the user has a password, prompt the user to validate the user's password.
                if (await _userManager.HasPasswordAsync(existingUser)) return Page();

                // If the user does not have a password, attach the external login to the user
                var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);

                if (addLoginResult.Succeeded)
                    return await LogInWithExternalProvider(info, false, true, returnUrl, OnExternalLoginFailed);

                return OnIdentityError(addLoginResult, returnUrl);
            }

            // If the user does not have an account, then create an account.
            var user = new ApplicationUser {UserName = UserName, IsLockedOut = true};
            var createUserResult = await _userManager.CreateAsync(user);

            if (createUserResult.Succeeded)
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded)
                    return await LogInWithExternalProvider(info, false, true, returnUrl, OnExternalLoginFailed);
            }

            return OnIdentityError(createUserResult, returnUrl);
        }

        private IActionResult OnIdentityError(IdentityResult result, string returnUrl)
        {
            ErrorMessage = result.Errors.First().Description;
            return RedirectToPage("./Login", new {ReturnUrl = returnUrl});
        }

        private Task<IActionResult> OnExternalLoginFailed(ExternalLoginInfo info, string returnUrl)
        {
            ErrorMessage = "There was an error adding your external login.";
            return Task.FromResult<IActionResult>(RedirectToPage("./Login", new {ReturnUrl = returnUrl}));
        }

        private string GetUserNameFromInfo(ExternalLoginInfo info)
        {
            var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email) ??
                            info.Principal.FindFirstValue(ClaimTypes.Name);
            return userEmail?.Split("@").FirstOrDefault();
        }

        private delegate Task<IActionResult> OnExternalLoginFail(ExternalLoginInfo info, string returnUrl);

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}