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
        }

        [BindProperty] public ScheduledMeetingTime ScheduledMeetingTime { get; set; }

        public CourseSection CourseSection { get; set; }
        
        public IEnumerable<SelectListItem> InstructorOptions { get; set; }
        public IEnumerable<SelectListItem> RoomOptions { get; set; }
        
        [Display(Name = "Instructors")]
        [BindProperty]
        public IEnumerable<Guid> InstructorIds { get; set; } = new List<Guid>();
        
        [Display(Name = "Rooms")]
        [BindProperty]
        public IEnumerable<Guid> RoomIds { get; set; } = new List<Guid>();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ScheduledMeetingTime = await Context.ScheduledMeetingTimes
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .Include(smt => smt.CourseSection)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ScheduledMeetingTime == null)
            {
                return NotFound();
            }
            
            InstructorIds = ScheduledMeetingTime.ScheduledMeetingTimeInstructors
                .Select(smti => smti.InstructorId);

            InstructorOptions = Context.Instructors
                .Where(i => i.IsActive || InstructorIds.Contains(i.Id))
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.FullName
                });
            
            RoomIds = ScheduledMeetingTime.ScheduledMeetingTimeRooms
                .Select(smtr => smtr.RoomId);

            RoomOptions = Context.Rooms
                .Include(r => r.Building)
                .Where(r => r.IsEnabled || RoomIds.Contains(r.Id))
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Building.Code + " " + r.Number
                });

            CourseSection = ScheduledMeetingTime.CourseSection;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var scheduledMeetingTime = await Context.ScheduledMeetingTimes
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .Include(smt => smt.CourseSection)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (scheduledMeetingTime == null)
            {
                return NotFound();
            }
            
            InstructorOptions = Context.Instructors
                .Where(i => i.IsActive || InstructorIds.Contains(i.Id))
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.FullName
                });

            RoomOptions = Context.Rooms
                .Include(r => r.Building)
                .Where(r => r.IsEnabled || RoomIds.Contains(r.Id))
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Building.Code + " " + r.Number
                });

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