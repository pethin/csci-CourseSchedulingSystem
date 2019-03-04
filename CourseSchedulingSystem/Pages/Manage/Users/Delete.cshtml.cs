using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(UserManager<ApplicationUser> userManager)
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
            if (id == null) return NotFound();

            ApplicationUser = await _userManager.FindByIdAsync(id.ToString());

            if (ApplicationUser != null)
            {
                var result = await _userManager.DeleteAsync(ApplicationUser);

                if (result.Succeeded) return RedirectToPage("./Index");

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}