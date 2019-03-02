using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Departments
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

        [BindProperty]
        public Department Department { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newDepartment = new Department();

            if (await TryUpdateModelAsync<Department>(
                newDepartment,
                "Department",
                d => d.Code, d => d.Name))
            {
                // Check if any department has the same code
                if (await _context.Departments.AnyAsync(d => d.Code == newDepartment.Code))
                {
                    ModelState.AddModelError(string.Empty, $"A department already exists with the code {newDepartment.Code}.");
                }

                // Check if any subject has the same name
                if (await _context.Departments.AnyAsync(d => d.NormalizedName == newDepartment.NormalizedName))
                {
                    ModelState.AddModelError(string.Empty, $"A department already exists with the name {newDepartment.Name}.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                _context.Departments.Add(newDepartment);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}