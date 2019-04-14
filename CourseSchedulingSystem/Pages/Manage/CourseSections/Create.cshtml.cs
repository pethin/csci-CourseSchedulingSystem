using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class CreateModel : CourseSectionsPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnGet(Guid? termId)
        {
            if (termId == null)
            {
                return NotFound();
            }

            Term = await Context.Terms.Where(t => t.Id == termId).FirstOrDefaultAsync();

            if (Term == null)
            {
                return NotFound();
            }

            TermPartIds = Context.TermParts
                .Where(tp => tp.TermId == Term.Id)
                .OrderBy(tp => tp.Name)
                .Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name + " | " + tp.StartDate.Value.ToString("MM/dd/yyyy") + " - " +
                           tp.EndDate.Value.ToString("MM/dd/yyyy")
                });

            return Page();
        }

        [BindProperty] public CourseSection CourseSection { get; set; }

        public Term Term { get; set; }

        public async Task<IActionResult> OnPostAsync(Guid? termId)
        {
            if (termId == null)
            {
                return NotFound();
            }

            Term = await Context.Terms.Where(t => t.Id == termId).FirstOrDefaultAsync();

            if (Term == null)
            {
                return NotFound();
            }

            TermPartIds = Context.TermParts
                .Where(tp => tp.TermId == Term.Id)
                .OrderBy(tp => tp.Name)
                .Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name + " | " + tp.StartDate.Value.ToString("MM/dd/yyyy") + " - " +
                           tp.EndDate.Value.ToString("MM/dd/yyyy")
                });

            if (!ModelState.IsValid) return Page();

            var courseSection = new CourseSection();

            if (await TryUpdateModelAsync(
                courseSection,
                "CourseSection",
                cs => cs.TermPartId,
                cs => cs.CourseId,
                cs => cs.Section,
                cs => cs.ScheduleTypeId,
                cs => cs.InstructionalMethodId,
                cs => cs.MaximumCapacity))
            {
                await courseSection.DbValidateAsync(Context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                Context.CourseSections.Add(courseSection);
                await Context.SaveChangesAsync();
                return RedirectToPage("./Edit", new {id = courseSection.Id});
            }

            return Page();
        }
    }
}