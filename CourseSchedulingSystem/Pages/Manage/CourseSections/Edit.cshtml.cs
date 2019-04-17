using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class EditModel : CourseSectionsPageModel
    {
        public EditModel(ApplicationDbContext context) : base(context)
        {
        }
        
        [FromRoute] public Guid Id { get; set; }

        [BindProperty]
        public CourseSection CourseSection { get; set; }

        public Term Term { get; set; }
        public IEnumerable<ScheduledMeetingTime> ScheduledMeetingTimes { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CourseSection = await Context.CourseSections.FirstOrDefaultAsync(m => m.Id == Id);

            if (CourseSection == null)
            {
                return NotFound();
            }

            Term = await Context.Terms
                .Include(t => t.TermParts)
                .Where(t => t.TermParts.Any(tp => tp.Id == CourseSection.TermPartId))
                .FirstOrDefaultAsync();
            
            TermPartIds = Context.TermParts
                .Where(tp => tp.TermId == Term.Id)
                .OrderBy(tp => tp.Name)
                .Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name + " | " + tp.StartDate.Value.ToString("MM/dd/yyyy") + " - " +
                           tp.EndDate.Value.ToString("MM/dd/yyyy")
                });

            ScheduledMeetingTimes = Context.ScheduledMeetingTimes
                .Where(smt => smt.CourseSectionId == Id)
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .ThenInclude(smti => smti.Instructor)
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .ThenInclude(smtr => smtr.Room)
                .ThenInclude(r => r.Building)
                .Include(smt => smt.MeetingType)
                .OrderBy(smt => smt.MeetingType.Code)
                .ThenBy(smt => smt.StartTime)
                .ThenBy(smt => smt.EndTime);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var courseSection = await Context.CourseSections.FirstOrDefaultAsync(m => m.Id == Id);

            if (courseSection == null) return NotFound();

            Term = await Context.Terms
                .Include(t => t.TermParts)
                .Where(t => t.TermParts.Any(tp => tp.Id == courseSection.TermPartId))
                .FirstOrDefaultAsync();
            
            TermPartIds = Context.TermParts
                .Where(tp => tp.TermId == Term.Id)
                .OrderBy(tp => tp.Name)
                .Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name + " | " + tp.StartDate.Value.ToString("MM/dd/yyyy") + " - " +
                           tp.EndDate.Value.ToString("MM/dd/yyyy")
                });

            ScheduledMeetingTimes = Context.ScheduledMeetingTimes
                .Where(smt => smt.CourseSectionId == courseSection.Id)
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .ThenInclude(smti => smti.Instructor)
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .ThenInclude(smtr => smtr.Room)
                .ThenInclude(r => r.Building)
                .Include(smt => smt.MeetingType)
                .OrderBy(smt => smt.MeetingType.Code)
                .ThenBy(smt => smt.StartTime)
                .ThenBy(smt => smt.EndTime);
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

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

                await Context.SaveChangesAsync();
                
                SuccessMessage = "Course section successfully updated!";
                return Page();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostDuplicateMeetingTimeAsync(Guid? meetingTimeId)
        {
            if (meetingTimeId == null)
            {
                ModelState.AddModelError(string.Empty, "Could not duplicate meeting time: missing ID.");
                return await OnGetAsync();
            }

            var scheduledMeetingTime = await Context.ScheduledMeetingTimes
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .Where(smt => smt.Id == meetingTimeId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (scheduledMeetingTime == null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Could not duplicate meeting time: could not find scheduled meeting time with ID {meetingTimeId}.");
                return await OnGetAsync();
            }
            
            scheduledMeetingTime.Id = Guid.NewGuid();
            
            scheduledMeetingTime.MeetingTypeId = MeetingType.AdditionalClassTimeMeetingType.Id;
            
            scheduledMeetingTime.ScheduledMeetingTimeRooms
                .ForEach(smtr => smtr.ScheduledMeetingTimeId = scheduledMeetingTime.Id);
            
            scheduledMeetingTime.ScheduledMeetingTimeInstructors
                .ForEach(smti => smti.ScheduledMeetingTimeId = scheduledMeetingTime.Id);

            Context.ScheduledMeetingTimes.Add(scheduledMeetingTime);
            await Context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
