using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections.ScheduledMeetingTimes
{
    public class DetailsModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public DetailsModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public ScheduledMeetingTime ScheduledMeetingTime { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ScheduledMeetingTime = await _context.ScheduledMeetingTimes
                .Include(s => s.CourseSection)
                .Include(s => s.MeetingType).FirstOrDefaultAsync(m => m.Id == id);

            if (ScheduledMeetingTime == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
