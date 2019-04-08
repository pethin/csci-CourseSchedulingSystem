using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DeleteModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty] public ApplicationUser ApplicationUser { get; set; }

        public bool CanDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ApplicationUser = await _context.Users
                .Include(u => u.DepartmentUsers)
                .ThenInclude(du => du.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ApplicationUser == null) return NotFound();

            CanDelete = await MoreThanOneActiveUser();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ApplicationUser = await _userManager.FindByIdAsync(id.ToString());

            if (ApplicationUser != null)
            {
                CanDelete = await MoreThanOneActiveUser();
                if (!CanDelete)
                {
                    return Page();
                }

                var result = await _userManager.DeleteAsync(ApplicationUser);

                if (result.Succeeded) return RedirectToPage("./Index");

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private async Task<bool> MoreThanOneActiveUser()
        {
            return await _context.Users
                       .Where(u => !u.IsLockedOut)
                       .CountAsync() > 1;
        }
    }
}