using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class EditModel : CoursesPageModel
    {
        public EditModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public Course Course { get; set; }
        
        [Display(Name = "Schedule Types")]
        [BindProperty]
        public IEnumerable<Guid> ScheduleTypeIds { get; set; } = new List<Guid>();

        [Display(Name = "Course Attributes")]
        [BindProperty]
        public IEnumerable<Guid> CourseAttributeIds { get; set; } = new List<Guid>();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Course = await Context.Courses
                .Include(c => c.CourseScheduleTypes)
                .Include(c => c.CourseCourseAttributes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Course == null) return NotFound();

            ScheduleTypeIds = Course.CourseScheduleTypes.Select(cst => cst.ScheduleTypeId);
            CourseAttributeIds = Course.CourseCourseAttributes.Select(cca => cca.CourseAttributeId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var course = await Context.Courses
                .Include(c => c.CourseScheduleTypes)
                .Include(c => c.CourseCourseAttributes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync(
                course,
                "Course",
                c => c.DepartmentId,
                c => c.SubjectId,
                c => c.Number,
                c => c.Title,
                c => c.CreditHours,
                c => c.IsUndergraduate,
                c => c.IsGraduate,
                c => c.IsEnabled))
            {
                await course.DbValidateAsync(Context).AddErrorsToModelState(ModelState);
                if (!ModelState.IsValid) return Page();

                // Update schedule types
                Context.UpdateManyToMany(course.CourseScheduleTypes,
                    ScheduleTypeIds
                        .Select(stId => new CourseScheduleType
                        {
                            CourseId = course.Id,
                            ScheduleTypeId = stId
                        }),
                    cst => cst.ScheduleTypeId);

                // Update course attributes
                Context.UpdateManyToMany(course.CourseCourseAttributes,
                    CourseAttributeIds
                        .Select(caId => new CourseCourseAttribute
                        {
                            CourseId = course.Id,
                            CourseAttributeId = caId
                        }),
                    cca => cca.CourseAttributeId);

                await Context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}