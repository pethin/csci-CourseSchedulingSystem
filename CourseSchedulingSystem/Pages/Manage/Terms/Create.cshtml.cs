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

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class CreateModel : TermsPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public TermInputModel Term { get; set; }

        public IActionResult OnGet()
        {
            Term = new TermInputModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Check if any term part have the same names
            Term.CheckForDuplicateTermParts(ModelState);
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

        public async Task<IActionResult> OnPostAddRowAsync()
        {
            await Task.Yield();

            if (Term.TermParts == null)
            {
                Term.TermParts = new List<TermPartInputModel>();
            }

            Term.TermParts.Add(new TermPartInputModel());

            ModelState.Clear();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteRowAsync(int row)
        {
            await Task.Yield();

            if (row < 0 || Term.TermParts.Count <= row)
            {
                return Page();
            }

            Term.TermParts.RemoveAt(row);

            ModelState.Clear();
            return Page();
        }
    }
}