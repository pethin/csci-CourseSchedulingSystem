using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CourseSchedulingSystem.Pages.Manage.Instructors
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public Instructor Instructor { get; set; }

        public IEnumerable<Course> Courses { get; set; }
        public bool InUse => Instructor.ScheduledMeetingTimeInstructors.Any();

        public async Task<IActionResult> OnGetAsync()
        {
            Instructor = await _context.Instructors
                .Include(i => i.ScheduledMeetingTimeInstructors)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Instructor == null) return NotFound();

            Courses = _context.Courses
                .Include(c => c.Subject)
                .Where(c => c.CourseSections.Any(cs =>
                    cs.ScheduledMeetingTimes.Any(smt =>
                        smt.ScheduledMeetingTimeInstructors.Any(smti =>
                            smti.InstructorId == Id))));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Instructor = await _context.Instructors.FindAsync(Id);

            if (Instructor != null)
            {
                if (InUse)
                {
                    return RedirectToPage();
                }
                
                _context.Instructors.Remove(Instructor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}