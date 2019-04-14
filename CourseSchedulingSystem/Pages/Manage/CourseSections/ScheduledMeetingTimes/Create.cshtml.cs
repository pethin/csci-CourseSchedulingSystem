using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections.ScheduledMeetingTimes
{
    public class CreateModel : ScheduledMeetingTimesPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnGet(Guid? sectionId)
        {
            if (sectionId == null) return NotFound();

            CourseSection = await Context.CourseSections
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(cs => cs.Id == sectionId);

            if (CourseSection == null) return NotFound();
            
            return Page();
        }

        [BindProperty]
        public ScheduledMeetingTime ScheduledMeetingTime { get; set; }

        public IEnumerable<SelectListItem> InstructorOptions;
        
        public CourseSection CourseSection { get; set; }

        public async Task<IActionResult> OnPostAsync(Guid? sectionId)
        {
            if (sectionId == null) return NotFound();

            CourseSection = await Context.CourseSections
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(cs => cs.Id == sectionId);

            if (CourseSection == null) return NotFound();

            if (!ModelState.IsValid) return Page();

            var scheduledMeetingTime = new ScheduledMeetingTime
            {
                CourseSectionId = CourseSection.Id
            };

            if (await TryUpdateModelAsync(
                scheduledMeetingTime,
                "ScheduledMeetingTime",
                smt => smt.MeetingTypeId,
                smt => smt.StartTime,
                smt => smt.EndTime,
                smt => smt.Monday,
                smt => smt.Tuesday,
                smt => smt.Wednesday,
                smt => smt.Thursday,
                smt => smt.Friday,
                smt => smt.Saturday,
                smt => smt.Sunday))
            {
                await scheduledMeetingTime.DbValidateAsync(Context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                Context.ScheduledMeetingTimes.Add(scheduledMeetingTime);
                await Context.SaveChangesAsync();
                return RedirectToPage("../Edit", new {id = CourseSection.Id});
            }

            return Page();
        }
    }
}