using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections.ScheduledMeetingTimes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ScheduledMeetingTime ScheduledMeetingTime { get; set; }
        
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }

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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ScheduledMeetingTime == null)
            {
                return NotFound();
            }
            
            if (returnUrl == null || !Url.IsLocalUrl(returnUrl))
            {
                ReturnUrl = Url.Page("../Details", new {id = ScheduledMeetingTime.CourseSectionId});
            }
            else
            {
                ReturnUrl = returnUrl;
            }
            
            return Page();
        }
    }
}
