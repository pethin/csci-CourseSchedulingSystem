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
using CourseSchedulingSystem.Utilities;

namespace CourseSchedulingSystem.Pages.Manage.Terms.TermParts
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public TermPart TermPart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            TermPart = await _context.TermParts
                .Include(t => t.Term)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (TermPart == null)
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

            var termPart = await _context.TermParts
                .Include(t => t.Term)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (await TryUpdateModelAsync(
                termPart,
                "TermPart",
                tp => tp.Name,
                tp => tp.StartDate,
                tp => tp.EndDate))
            {
                await termPart.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("../Edit", new {id = termPart.TermId});
            }

            return Page();
        }
    }
}