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
    public class EditModel : TermsPageModel
    {
        public EditModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public TermInputModel Term { get; set; }

        [BindProperty] public TermPartInputModel TermPart { get; set; }

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

        public async Task<IActionResult> OnPostUpdateNameAsync(Guid? id)
        {
            // Only validate Term
            ModelState.Clear();
            TryValidateModel(Term);

            if (!ModelState.IsValid) return Page();

            var term = await Context.Terms.FindAsync(id);

            if (await TryUpdateModelAsync(
                term,
                "Term",
                t => t.Name))
            {
                await term.DbValidateAsync(Context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                await Context.SaveChangesAsync();
                return await OnGetAsync(id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddPartAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var term = await Context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (term == null) return NotFound();

            // Only validate TermPart
            ModelState.Clear();
            TryValidateModel(TermPart, "TermPart");

            if (!ModelState.IsValid) await OnGetAsync(id);

            var termPart = new TermPart
            {
                TermId = term.Id
            };

            if (await TryUpdateModelAsync(termPart, "TermPart", tp => tp.Name, tp => tp.StartDate, tp => tp.EndDate))
            {
                await termPart.DbValidateAsync(Context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return await OnGetAsync(id);

                Context.TermParts.Add(termPart);
                await Context.SaveChangesAsync();

                // Reset form
                TermPart = new TermPartInputModel();
                ModelState.Clear();
            }

            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostUpdatePartAsync(Guid? id, int partIndex)
        {
            if (id == null) return NotFound();

            var modelStateKey = $"Term.TermParts[{partIndex}]";

            // Only validate TermPart
            ModelState.Clear();
            TryValidateModel(Term.TermParts[partIndex], modelStateKey);

            if (!ModelState.IsValid) await OnGetAsync(id);

            var termPart = await Context.TermParts.FindAsync(Term.TermParts[partIndex].Id);

            if (await TryUpdateModelAsync(termPart, modelStateKey, tp => tp.Name, tp => tp.StartDate, tp => tp.EndDate))
            {
                await termPart.DbValidateAsync(Context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(modelStateKey + ".Name", result.ErrorMessage);
                });

                if (!ModelState.IsValid) return await OnGetAsync(id);

                await Context.SaveChangesAsync();
            }

            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostDeletePartAsync(Guid? id, Guid partId)
        {
            if (id == null) return NotFound();

            var termPart = await Context.TermParts.FindAsync(partId);

            // TODO: Check if in use
            Context.TermParts.Remove(termPart);
            await Context.SaveChangesAsync();

            ModelState.Clear();

            return await OnGetAsync(id);
        }
    }
}