using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms.TermParts
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(Guid? termId)
        {
            if (termId == null) return NotFound();

            Term = await _context.Terms.FirstOrDefaultAsync(t => t.Id == termId);

            if (Term == null) return NotFound();

            return Page();
        }

        public Term Term { get; set; }

        [BindProperty] public TermPart TermPart { get; set; }

        public async Task<IActionResult> OnPostAsync(Guid? termId)
        {
            if (termId == null) return NotFound();

            Term = await _context.Terms.FirstOrDefaultAsync(t => t.Id == termId);

            if (Term == null) return NotFound();

            if (!ModelState.IsValid) return Page();

            var newTermPart = new TermPart
            {
                TermId = Term.Id
            };

            if (await TryUpdateModelAsync(
                newTermPart,
                "TermPart",
                tp => tp.Name, tp => tp.StartDate, tp => tp.EndDate))
            {
                // Check if any term part has the same name
                if (await _context.TermParts.AnyAsync(tp =>
                    tp.TermId == newTermPart.TermId && tp.Name == newTermPart.Name))
                    ModelState.AddModelError(string.Empty,
                        $"A part of term already exists with the name {newTermPart.Name}.");

                if (!ModelState.IsValid) return Page();

                _context.TermParts.Add(newTermPart);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new { termId = Term.Id });
            }

            return Page();
        }
    }
}