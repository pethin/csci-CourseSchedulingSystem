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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Term Term { get; set; }

        [BindProperty] public TermPart TermPart { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? termId)
        {
            if (termId == null) return NotFound();

            Term = await _context.Terms.FirstOrDefaultAsync(t => t.Id == termId);

            if (Term == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? termId)
        {
            if (termId == null) return NotFound();

            Term = await _context.Terms.FirstOrDefaultAsync(t => t.Id == termId);

            if (Term == null) return NotFound();

            if (!ModelState.IsValid) return Page();

            var termPart = new TermPart
            {
                Term = Term,
                TermId = Term.Id
            };

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

                _context.TermParts.Add(termPart);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new {termId = Term.Id});
            }

            return Page();
        }
    }
}