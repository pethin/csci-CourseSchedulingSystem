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

        public Course Course { get; set; }
        public IEnumerable<Instructor> Instructors { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Course = await Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .Include(c => c.CourseScheduleTypes)
                .ThenInclude(cst => cst.ScheduleType)
                .Include(c => c.CourseCourseAttributes)
                .ThenInclude(cat => cat.CourseAttribute)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Course == null) return NotFound();

            Instructors = Context.Instructors
                .Where(i => i.ScheduledMeetingTimeInstructors.Any(smti =>
                    smti.ScheduledMeetingTime.CourseSection.CourseId == id));

            return Page();
        }
    }
}