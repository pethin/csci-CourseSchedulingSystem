using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections.ScheduledMeetingTimes
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public ScheduledMeetingTime ScheduledMeetingTime { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ScheduledMeetingTime = await _context.ScheduledMeetingTimes
                .Include(s => s.CourseSection)
                .ThenInclude(cs => cs.Course)
                .ThenInclude(c => c.Subject)
                .Include(s => s.MeetingType)
                .Include(smt => smt.ScheduledMeetingTimeInstructors)
                .ThenInclude(smti => smti.Instructor)
                .Include(smt => smt.ScheduledMeetingTimeRooms)
                .ThenInclude(smtr => smtr.Room)
                .ThenInclude(r => r.Building)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (ScheduledMeetingTime == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ScheduledMeetingTime = await _context.ScheduledMeetingTimes.FindAsync(Id);

            if (ScheduledMeetingTime == null)
            {
                return NotFound();
            }

            _context.ScheduledMeetingTimes.Remove(ScheduledMeetingTime);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Edit", new {id = ScheduledMeetingTime.CourseSectionId});
        }
    }
}