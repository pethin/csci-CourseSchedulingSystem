using System;
using System.Collections.Async;
using System.Collections.Generic;
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

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public Term Term { get; set; }
        
        public string OriginalTermName { get; set; }
        public IEnumerable<TermPart> TermParts { get; set; }

        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Term = await _context.Terms.FirstOrDefaultAsync(m => m.Id == Id);

            if (Term == null) return NotFound();

            OriginalTermName = Term.Name;
            TermParts = _context.TermParts.Where(tp => tp.TermId == Term.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var term = await _context.Terms.FirstOrDefaultAsync(m => m.Id == Id);

            if (term == null)
            {
                return NotFound();
            }
            
            OriginalTermName = term.Name;
            TermParts = _context.TermParts.Where(tp => tp.TermId == term.Id);

            if (await TryUpdateModelAsync(
                term,
                "Term",
                s => s.Name))
            {
                await term.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();

                OriginalTermName = term.Name;
                SuccessMessage = "Name successfully updated!";
                return Page();
            }

            return Page();
        }
    }
}