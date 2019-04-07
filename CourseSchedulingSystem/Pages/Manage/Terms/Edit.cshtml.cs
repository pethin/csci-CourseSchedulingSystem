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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Term Term { get; set; }

        public string SourceTermName { get; set; }

        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Term == null) return NotFound();

            SourceTermName = Term.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Term != null)
            {
                SourceTermName = Term.Name;
            }

            if (await TryUpdateModelAsync(
                Term,
                "Term",
                s => s.Name))
            {
                await Term.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();

                SuccessMessage = "Name successfully updated!";
                return Page();
            }

            return Page();
        }
    }
}