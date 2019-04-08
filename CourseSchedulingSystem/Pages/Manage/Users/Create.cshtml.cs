using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CreateModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

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

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = new ApplicationUser();

            if (await TryUpdateModelAsync(
                user,
                "ApplicationUser",
                u => u.UserName, u => u.IsLockedOut))
            {
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    // Add departments
                    _context.DepartmentUsers.AddRange(DepartmentIds.Select(dId => new DepartmentUser
                    {
                        UserId = user.Id,
                        DepartmentId = dId
                    }));

                    await _context.SaveChangesAsync();
                    
                    return RedirectToPage("./Index");
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}