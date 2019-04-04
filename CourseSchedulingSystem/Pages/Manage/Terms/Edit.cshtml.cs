using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class EditModel : TermsPageModel
    {
        public EditModel(ApplicationDbContext context) : base(context)
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

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            // TODO: Actually make it do what it's supposed to do
            if (!ModelState.IsValid) return Page();

            var term = await Context.Terms.FindAsync(id);

            if (await TryUpdateModelAsync(
                term,
                "Term",
                t => t.Name, t => t.StartDate, t => t.EndDate))
            {
                await term.DbValidateAsync(Context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                await Context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
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
