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
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Users
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User UserModel { get; set; }

        [Display(Name = "Departments")]
        [BindProperty]
        public IEnumerable<Guid> DepartmentIds { get; set; }

        public IEnumerable<SelectListItem> DepartmentOptions => _context.Departments
            .OrderBy(d => d.NormalizedName)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            });

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserModel = await _context.Users
                .Include(u => u.DepartmentUsers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (User == null)
            {
                return NotFound();
            }

            DepartmentIds = UserModel.DepartmentUsers.Select(du => du.DepartmentId);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users
                .Include(u => u.DepartmentUsers)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (await TryUpdateModelAsync(
                user,
                "User",
                u => u.UserName, u => u.IsLockedOut))
            {
                await user.DbValidateAsync(_context).AddErrorsToModelState(ModelState);
                if (!ModelState.IsValid) return Page();

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

            return Page();
        }
    }
}
