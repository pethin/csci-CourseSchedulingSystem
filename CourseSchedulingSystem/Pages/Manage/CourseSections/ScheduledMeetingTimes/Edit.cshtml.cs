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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections.ScheduledMeetingTimes
{
    public class EditModel : ScheduledMeetingTimesPageModel
    {
        public EditModel(ApplicationDbContext context) : base(context)
        {
            InstructorIds = ScheduledMeetingTime.ScheduledMeetingTimeInstructors
                .Select(smti => smti.InstructorId);

            RoomIds = ScheduledMeetingTime.ScheduledMeetingTimeRooms
                .Select(smtr => smtr.RoomId);
        }

        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public ScheduledMeetingTime ScheduledMeetingTime { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ScheduledMeetingTime = await Context.ScheduledMeetingTimes
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .Include(smt => smt.CourseSection)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (ScheduledMeetingTime == null)
            {
                return NotFound();
            }

            CourseSection = ScheduledMeetingTime.CourseSection;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var scheduledMeetingTime = await Context.ScheduledMeetingTimes
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .Include(smt => smt.CourseSection)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (scheduledMeetingTime == null)
            {
                return NotFound();
            }

            CourseSection = scheduledMeetingTime.CourseSection;

            if (!ModelState.IsValid)
            {
                return Page();
            }

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

                // Update instructors
                Context.UpdateManyToMany(scheduledMeetingTime.ScheduledMeetingTimeInstructors,
                    InstructorIds
                        .Select(instructorId => new ScheduledMeetingTimeInstructor
                        {
                            ScheduledMeetingTimeId = scheduledMeetingTime.Id,
                            InstructorId = instructorId
                        }),
                    smti => smti.InstructorId);

                // Update rooms
                Context.UpdateManyToMany(scheduledMeetingTime.ScheduledMeetingTimeRooms,
                    RoomIds
                        .Select(roomId => new ScheduledMeetingTimeRoom
                        {
                            ScheduledMeetingTimeId = scheduledMeetingTime.Id,
                            RoomId = roomId
                        }),
                    smtr => smtr.RoomId);

                await Context.SaveChangesAsync();
                return RedirectToPage("../Edit", new {id = scheduledMeetingTime.CourseSectionId});
            }

            return Page();
        }
    }
}