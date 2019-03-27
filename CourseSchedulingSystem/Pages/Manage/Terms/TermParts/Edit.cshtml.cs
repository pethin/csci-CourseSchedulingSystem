using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms.TermParts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public TermPart TermPart { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            TermPart = await _context.TermParts
                .Include(tp => tp.Term)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (TermPart == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var termPart = await _context.TermParts
                .Include(tp => tp.Term)
                .FirstOrDefaultAsync(tp => tp.Id == id);

            if (await TryUpdateModelAsync(
                termPart,
                "TermPart",
                tp => tp.Name, tp => tp.StartDate, tp => tp.EndDate))
            {
                await termPart.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new {termId = termPart.TermId});
            }

            return Page();
        }
    }
}