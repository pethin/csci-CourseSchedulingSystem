using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
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

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public Department Department { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == Id);

            if (Department == null) return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var department = await _context.Departments.FindAsync(Id);

            if (await TryUpdateModelAsync(
                department,
                "Department",
                d => d.Code, d => d.Name))
            {
                await department.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}