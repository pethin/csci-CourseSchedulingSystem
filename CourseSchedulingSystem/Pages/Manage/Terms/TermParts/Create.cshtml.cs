using System;
using System.Collections.Async;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
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

        [FromRoute] public Guid TermId { get; set; }
        
        [BindProperty] public TermPart TermPart { get; set; }

        public Term Term { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Term = await _context.Terms.FirstOrDefaultAsync(t => t.Id == TermId);

            if (Term == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Term = await _context.Terms.FirstOrDefaultAsync(t => t.Id == TermId);

            if (Term == null) return NotFound();

            if (!ModelState.IsValid) return Page();

            var termPart = new TermPart
            {
                TermId = Term.Id
            };

            if (await TryUpdateModelAsync(
                termPart,
                "TermPart",
                tp => tp.Name,
                tp => tp.StartDate,
                tp => tp.EndDate))
            {
                await termPart.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                _context.TermParts.Add(termPart);
                await _context.SaveChangesAsync();
                return RedirectToPage("../Edit", new {id = Term.Id});
            }

            return Page();
        }
    }
}