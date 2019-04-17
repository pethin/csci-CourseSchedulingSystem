using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class CreateModel : CourseSectionsPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context)
        {
        }

        [FromRoute] public Guid TermId { get; set; }
        
        [BindProperty] public CourseSection CourseSection { get; set; }

        public Term Term { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Term = await Context.Terms.Where(t => t.Id == TermId).FirstOrDefaultAsync();

            if (Term == null) return NotFound();

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

        public async Task<IActionResult> OnGetNextSectionNumberAsync(Guid? courseId)
        {
            if (courseId == null)
            {
                return new JsonResult(new {message = "Missing required parameter courseId."})
                    {StatusCode = (int) HttpStatusCode.NotFound};
            }

            var course = await Context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return new JsonResult(new {message = "Course not found."})
                    {StatusCode = (int) HttpStatusCode.NotFound};
            }

            // Ignore contract courses and restricted courses
            int nextSectionNumber = await Context.CourseSections
                .Include(cs => cs.TermPart)
                .Where(cs => cs.TermPart.TermId == TermId)
                .Where(cs => cs.CourseId == courseId)
                .Where(cs => (cs.Section < 80) || (cs.Section >= 90 && cs.Section < 600) || (cs.Section >= 700))
                .OrderByDescending(cs => cs.Section)
                .Select(cs => cs.Section)
                .FirstOrDefaultAsync() + 1;

            return new JsonResult(nextSectionNumber);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Term = await Context.Terms.Where(t => t.Id == TermId).FirstOrDefaultAsync();

            if (Term == null) return NotFound();

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