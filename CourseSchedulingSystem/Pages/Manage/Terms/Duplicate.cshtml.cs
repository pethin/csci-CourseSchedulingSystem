using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class DuplicateModel : TermsPageModel
    {
        public DuplicateModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public TermInputModel Term { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var term = await Context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (term == null) return NotFound();

            Term = new TermInputModel
            {
                Id = term.Id,
                Name = term.Name,
                TermParts = term.TermParts.Select(tp => new TermPartInputModel
                    {
                        Id = tp.Id,
                        Name = tp.Name,
                        StartDate = tp.StartDate,
                        EndDate = tp.EndDate
                    }
                ).ToList()
            };
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validate that no other term exists in the DB with the same name
            var term = new Term
            {
                Name = Term.Name
            };

            await term.DbValidateAsync(Context).AddErrorsToModelState(ModelState);

            if (!ModelState.IsValid) return Page();

            // Create the term
            Context.Terms.Add(term);
            await Context.SaveChangesAsync();

            // Create the term parts
            foreach (var termPart in Term.TermParts)
            {
                Debug.Assert(termPart.StartDate != null, "termPart.StartDate != null");
                Debug.Assert(termPart.EndDate != null, "tp.EndDate != null");
                
                Context.TermParts.Add(new TermPart
                {
                    TermId = term.Id,
                    Name = termPart.Name,
                    StartDate = (DateTime) termPart.StartDate,
                    EndDate = (DateTime) termPart.EndDate
                });
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}