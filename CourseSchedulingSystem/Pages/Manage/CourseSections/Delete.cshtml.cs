using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseSections
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public CourseSection CourseSection { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CourseSection = await _context.CourseSections
                .Include(c => c.TermPart)
                .ThenInclude(tp => tp.Term)
                .Include(c => c.Course)
                .ThenInclude(c => c.Subject)
                .Include(c => c.InstructionalMethod)
                .Include(c => c.ScheduleType)
                .Include(c => c.ScheduledMeetingTimes)
                .ThenInclude(smt => smt.MeetingType)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (CourseSection == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CourseSection = await _context.CourseSections
                .Include(cs => cs.TermPart)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (CourseSection == null)
            {
                return NotFound();
            }

            _context.CourseSections.Remove(CourseSection);
            await _context.SaveChangesAsync();
            
            // TODO: Recalculate SchedulingNotifications

            return RedirectToPage("./Index", new {termId = CourseSection.TermPart.TermId});
        }
    }
}