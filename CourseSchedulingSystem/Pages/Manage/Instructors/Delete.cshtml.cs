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

        [BindProperty] public Instructor Instructor { get; set; }

        public IEnumerable<Course> Courses { get; set; }
        public bool InUse => Instructor.ScheduledMeetingTimeInstructors.Any();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Instructor = await _context.Instructors
                .Include(i => i.ScheduledMeetingTimeInstructors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Instructor == null) return NotFound();

            Courses = _context.Courses
                .Include(c => c.Subject)
                .Where(c => c.CourseSections.Any(cs =>
                    cs.ScheduledMeetingTimes.Any(smt =>
                        smt.ScheduledMeetingTimeInstructors.Any(smti =>
                            smti.InstructorId == id))));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Instructor = await _context.Instructors.FindAsync(id);

            if (Instructor != null)
            {
                if (InUse)
                {
                    return await OnGetAsync(id);
                }
                
                _context.Instructors.Remove(Instructor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}