using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Departments
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);

            if (Department == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var departmentToUpdate = await _context.Departments.FindAsync(id);

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate,
                "Department",
                d => d.Code, d => d.Name))
            {
                // Check if any other department has the same name
                if (await _context.Departments.AnyAsync(d =>
                    d.Id != departmentToUpdate.Id && d.NormalizedName == departmentToUpdate.NormalizedName))
                {
                    ModelState.AddModelError(string.Empty,
                        $"A department already exists with the name {departmentToUpdate.Name}.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
