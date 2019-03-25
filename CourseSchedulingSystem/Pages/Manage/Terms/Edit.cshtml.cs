using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Term Term { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Term = await _context.Terms.FirstOrDefaultAsync(m => m.Id == id);

            if (Term == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var termToUpdate = await _context.Terms.FindAsync(id);

            if (await TryUpdateModelAsync(
                termToUpdate,
                "Term",
                t => t.Name, t => t.StartDate, t => t.EndDate))
            {
                // Check if any other term has the same name
                if (await _context.Terms.AnyAsync(t => t.Id != termToUpdate.Id && t.Name == termToUpdate.Name))
                    ModelState.AddModelError(string.Empty,
                        $"A term already exists with the name {termToUpdate.Name}.");

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
