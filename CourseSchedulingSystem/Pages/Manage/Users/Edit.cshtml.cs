using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty] public ApplicationUser ApplicationUser { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ApplicationUser = await _userManager.FindByIdAsync(id.ToString());

            if (ApplicationUser == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var userToUpdate = await _userManager.FindByIdAsync(id.ToString());

            if (await TryUpdateModelAsync(
                userToUpdate,
                "ApplicationUser",
                u => u.UserName, u => u.IsLockedOut))
            {
                var result = await _userManager.UpdateAsync(userToUpdate);

                if (result.Succeeded) return RedirectToPage("./Index");

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}