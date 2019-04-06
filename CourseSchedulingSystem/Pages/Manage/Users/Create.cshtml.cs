using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public User User { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!DepartmentIds.Any())
            {
                ModelState.AddModelError("DepartmentIds", "A department is required.");
                return Page();
            }

            var user = new User();

            if (await TryUpdateModelAsync(
                user,
                "User",
                u => u.UserName, u => u.IsLockedOut))
            {
                await user.DbValidateAsync(_context).AddErrorsToModelState(ModelState);
                if (!ModelState.IsValid) return Page();

                _context.Users.Add(user);

                _context.DepartmentUsers.AddRange(DepartmentIds.Select(dId => new DepartmentUser
                {
                    UserId = user.Id,
                    DepartmentId = dId
                }));

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}