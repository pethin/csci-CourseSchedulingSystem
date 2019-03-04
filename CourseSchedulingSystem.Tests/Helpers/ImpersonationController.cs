using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseSchedulingSystem.Tests.Helpers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImpersonationController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ImpersonationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] InputModel input)
        {
            var user = await _userManager.FindByIdAsync(input.Id.ToString());

            if (user == null) return NotFound(new {message = "User not found"});

            await _signInManager.SignInAsync(user, false);

            return Ok();
        }

        public class InputModel
        {
            [BindRequired] public Guid Id { get; set; }
        }
    }
}