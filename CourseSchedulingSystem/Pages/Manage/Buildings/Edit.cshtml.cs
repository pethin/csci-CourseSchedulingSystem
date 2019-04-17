using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Buildings
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty]
        public Building Building { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Building = await _context.Buildings.FirstOrDefaultAsync(m => m.Id == Id);

            if (Building == null)
            {
                return NotFound();
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var building = await _context.Buildings.FindAsync(Id);

            if (await TryUpdateModelAsync(building, "Building", bd => bd.Code, bd => bd.Name))
            {
                await building.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
