using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class DetailsModel : CoursesPageModel
    {
        public DetailsModel(ApplicationDbContext context) : base(context)
        {
        }

        [FromRoute] public Guid Id { get; set; }
        
        public Course Course { get; set; }
        
        public List<Instructor> Instructors { get; set; }

        public List<Term> Terms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Course = await Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .Include(c => c.CourseScheduleTypes)
                .ThenInclude(cst => cst.ScheduleType)
                .Include(c => c.CourseCourseAttributes)
                .ThenInclude(cat => cat.CourseAttribute)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Course == null) return NotFound();

            Instructors = await Context.Instructors
                .Where(i => i.ScheduledMeetingTimeInstructors.Any(smti =>
                    smti.ScheduledMeetingTime.CourseSection.CourseId == Id))
                .ToListAsync();

            Terms = await Context.Terms
                .Where(t => t.TermParts.Any(tp => tp.CourseSections.Any(cs => cs.CourseId == Id)))
                .ToListAsync();

            return Page();
        }
    }
}