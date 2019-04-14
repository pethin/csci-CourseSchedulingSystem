using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace CourseSchedulingSystem.Services.TestingAuthentication
{
    public class TestingAuthenticationHandler<TKey, TUser, TGroup> : AuthenticationHandler<TestingAuthenticationOptions>
        where TKey : IEquatable<TKey>
        where TUser : IdentityUser<TKey>, new()
        where TGroup : IdentityRole<TKey>, new()
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string BasicSchemeName = "Basic";
        
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;

        public TestingAuthenticationHandler(
            IOptionsMonitor<TestingAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            SignInManager<TUser> signInManager,
            UserManager<TUser> userManager)
            : base(options, logger, encoder, clock)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName],
                out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!BasicSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not Basic authentication header
                return AuthenticateResult.NoResult();
            }

            byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
            string userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
            string[] parts = userAndPassword.Split(':');
            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid Basic authentication header");
            }

            string userName = parts[0];

            // Try to find the user
            var user = await _userManager.FindByNameAsync(userName);

            // If the user doesn't exist create the user
            if (user == null)
            {
                user = new TUser
                {
                    UserName = userName
                };
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return AuthenticateResult.Fail($"Could not create user with username: {userName}");
                }
            }

            var identity = await _signInManager.ClaimsFactory.CreateAsync(user);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}