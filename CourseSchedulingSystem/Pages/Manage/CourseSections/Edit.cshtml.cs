using System;
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

        [BindProperty]
        public CourseSection CourseSection { get; set; }
        
        public Term Term { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            CourseSection = await Context.CourseSections
                .Include(c => c.TermPart)
                .ThenInclude(tp => tp.Term)
                .Include(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.MeetingType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (CourseSection == null)
            {
                return NotFound();
            }
            
            TermPartIds = Context.TermParts
                .Where(tp => tp.TermId == CourseSection.TermPart.TermId)
                .OrderBy(tp => tp.Name)
                .Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name + " | " + tp.StartDate.Value.ToString("MM/dd/yyyy") + " - " +
                           tp.EndDate.Value.ToString("MM/dd/yyyy")
                });
            
            Term = CourseSection.TermPart.Term;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null) return NotFound();
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            CourseSection = await Context.CourseSections
                .Include(cs => cs.TermPart)
                .ThenInclude(tp => tp.Term)
                .Include(cs => cs.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.MeetingType)
                .Where(cs => cs.Id == id)
                .FirstOrDefaultAsync();

            if (CourseSection == null) return NotFound();

            Term = CourseSection.TermPart.Term;
            
            TermPartIds = Context.TermParts
                .Where(tp => tp.TermId == CourseSection.TermPart.TermId)
                .OrderBy(tp => tp.Name)
                .Select(tp => new SelectListItem
                {
                    Value = tp.Id.ToString(),
                    Text = tp.Name + " | " + tp.StartDate.Value.ToString("MM/dd/yyyy") + " - " +
                           tp.EndDate.Value.ToString("MM/dd/yyyy")
                });

            if (await TryUpdateModelAsync(
                CourseSection,
                "CourseSection",
                cs => cs.TermPartId,
                cs => cs.CourseId,
                cs => cs.Section,
                cs => cs.ScheduleTypeId,
                cs => cs.InstructionalMethodId,
                cs => cs.MaximumCapacity))
            {
                await CourseSection.DbValidateAsync(Context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await Context.SaveChangesAsync();
                
                SuccessMessage = "Course section successfully updated!";
                return Page();
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostDuplicateMeetingTimeAsync(Guid? id, Guid? meetingTimeId)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (meetingTimeId == null)
            {
                ModelState.AddModelError(string.Empty, "Could not duplicate meeting time: missing ID.");
                return await OnGetAsync(id);
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
                return await OnGetAsync(id);
            }
            
            scheduledMeetingTime.Id = Guid.NewGuid();
            
            scheduledMeetingTime.MeetingTypeId = MeetingType.AdditionalClassTimeMeetingType.Id;
            
            scheduledMeetingTime.ScheduledMeetingTimeRooms
                .ForEach(smtr => smtr.ScheduledMeetingTimeId = scheduledMeetingTime.Id);
            
            scheduledMeetingTime.ScheduledMeetingTimeInstructors
                .ForEach(smti => smti.ScheduledMeetingTimeId = scheduledMeetingTime.Id);

            Context.ScheduledMeetingTimes.Add(scheduledMeetingTime);
            await Context.SaveChangesAsync();

            return await OnGetAsync(id);
        }
    }
}
