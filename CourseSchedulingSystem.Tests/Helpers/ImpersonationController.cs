using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace CourseSchedulingSystem.Tests.Helpers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImpersonationController : ControllerBase
    {
        private readonly IdentityOptions _options;

        public ImpersonationController(IOptions<IdentityOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public class InputModel
        {
            public Guid Id { get; set; }

            [BindRequired] public string UserName { get; set; }

            public string[] Roles { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] InputModel input)
        {
            var claims = new List<Claim>
            {
                new Claim(_options.ClaimsIdentity.UserIdClaimType, input.Id.ToString()),
                new Claim(_options.ClaimsIdentity.UserNameClaimType, input.UserName)
            };

            if (input.Roles != null)
            {
                foreach (var role in input.Roles)
                {
                    claims.Add(new Claim(_options.ClaimsIdentity.RoleClaimType, role));
                }
            }

            var claimsIdentity = new ClaimsIdentity(
                claims,
                IdentityConstants.ApplicationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok();
        }
    }
}