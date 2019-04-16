using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Instructors
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Instructor Instructor { get; set; }
        public IEnumerable<Course> Courses { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.Id == id);

            if (Instructor == null) return NotFound();
            
            Courses = _context.Courses
                .Include(c => c.Subject)
                .Where(c => c.CourseSections.Any(cs =>
                    cs.ScheduledMeetingTimes.Any(smt =>
                        smt.ScheduledMeetingTimeInstructors.Any(smti =>
                            smti.InstructorId == id))));
            
            return Page();
        }
    }
}