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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ImpersonationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public class InputModel
        {
            [BindRequired]
            public Guid Id { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] InputModel input)
        {
            var user = await _userManager.FindByIdAsync(input.Id.ToString());

            if (user == null)
            {
                return NotFound(new {message = "User not found"});
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok();
        }
    }
}