using System;
using System.Collections.Async;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class DuplicateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DuplicateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Term Term { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Term == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            Term = await _context.Terms
                .Include(t => t.TermParts)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            Term.Id = new Guid();

            if (await TryUpdateModelAsync(
                Term,
                "Term",
                s => s.Name))
            {
                await Term.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                // TODO: Also duplicate courses
                Term.TermParts.ForEach(part =>
                {
                    part.Id = new Guid();
                    part.TermId = Term.Id;
                });

                _context.Terms.Add(Term);

                await _context.SaveChangesAsync();

                return RedirectToPage("Edit", new {id = Term.Id});
            }

            return Page();
        }
    }
}