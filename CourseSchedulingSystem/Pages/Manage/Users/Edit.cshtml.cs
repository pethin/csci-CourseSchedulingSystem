using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Departments")]
        [BindProperty]
        public IEnumerable<Guid> DepartmentIds { get; set; } = new List<Guid>();

        public IEnumerable<SelectListItem> DepartmentOptions => _context.Departments
            .OrderBy(d => d.NormalizedName)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            });

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser = await _context.Users
                .Include(u => u.DepartmentUsers)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (ApplicationUser == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _context.Users
                .Include(u => u.DepartmentUsers)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (await TryUpdateModelAsync(
                user,
                "ApplicationUser",
                u => u.UserName, u => u.IsLockedOut))
            {
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update departments
                    _context.UpdateManyToMany(user.DepartmentUsers,
                        DepartmentIds
                            .Select(dId => new DepartmentUser
                            {
                                UserId = user.Id,
                                DepartmentId = dId
                            }),
                        du => du.DepartmentId);

                    await _context.SaveChangesAsync();

                    return RedirectToPage("./Index");
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}