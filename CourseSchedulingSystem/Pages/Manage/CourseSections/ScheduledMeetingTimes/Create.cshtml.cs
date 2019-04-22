using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [FromRoute] public Guid SectionId { get; set; }

        [BindProperty] public ScheduledMeetingTime ScheduledMeetingTime { get; set; }

        public async Task<IActionResult> OnGet()
        {
            CourseSection = await Context.CourseSections
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(cs => cs.Id == SectionId);

            if (CourseSection == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CourseSection = await Context.CourseSections
                .Include(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(cs => cs.Id == SectionId);

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

                // Add instructors
                Context.ScheduledMeetingTimeInstructors.AddRange(InstructorIds.Select(iId =>
                    new ScheduledMeetingTimeInstructor
                    {
                        ScheduledMeetingTimeId = scheduledMeetingTime.Id,
                        InstructorId = iId
                    }));

                // Add rooms
                Context.ScheduledMeetingTimeRooms.AddRange(RoomIds.Select(rId =>
                    new ScheduledMeetingTimeRoom
                    {
                        ScheduledMeetingTimeId = scheduledMeetingTime.Id,
                        RoomId = rId
                    }));

                await Context.SaveChangesAsync();
                
                // TODO: Recalculate SchedulingNotifications
                
                return RedirectToPage("../Edit", new {id = CourseSection.Id});
            }

            return Page();
        }
    }
}