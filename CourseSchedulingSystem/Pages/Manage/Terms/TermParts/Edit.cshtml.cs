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

namespace CourseSchedulingSystem.Pages.Manage.Terms.TermParts
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TermPart TermPart { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TermPart = await _context.TermParts
                .Include(t => t.Term)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (TermPart == null)
            {
                return NotFound();
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var termPartToUpdate = await _context.TermParts.FindAsync(id);

            if (await TryUpdateModelAsync(
                termPartToUpdate,
                "TermPart",
                tp => tp.Name, tp => tp.StartDate, tp => tp.EndDate))
            {
                // Check if any term part has the same name
                if (await _context.TermParts.AnyAsync(tp =>
                    tp.Id != termPartToUpdate.Id && tp.Name == termPartToUpdate.Name))
                    ModelState.AddModelError(string.Empty,
                        $"A part of term already exists with the name {termPartToUpdate.Name}.");

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new { termId = termPartToUpdate.TermId });
            }

            return Page();
        }
    }
}
