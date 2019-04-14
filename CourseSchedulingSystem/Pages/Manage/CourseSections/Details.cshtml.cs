using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public CourseSection CourseSection { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CourseSection = await _context.CourseSections
                .Include(c => c.TermPart)
                .ThenInclude(tp => tp.Term)
                .Include(c => c.Course)
                .ThenInclude(c => c.Subject)
                .Include(c => c.InstructionalMethod)
                .Include(c => c.ScheduleType)
                .Include(c => c.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.MeetingType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (CourseSection == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
