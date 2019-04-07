using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class CreateModel : CoursesPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public Course Course { get; set; }

        [Display(Name = "Schedule Types")]
        [BindProperty]
        public IEnumerable<Guid> ScheduleTypeIds { get; set; } = new List<Guid>();

        [Display(Name = "Course Attributes")]
        [BindProperty]
        public IEnumerable<Guid> CourseAttributeIds { get; set; } = new List<Guid>();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var course = new Course();

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

                Context.Courses.Add(course);

                Context.CourseScheduleTypes.AddRange(ScheduleTypeIds.Select(stId => new CourseScheduleType
                {
                    CourseId = course.Id,
                    ScheduleTypeId = stId
                }));

                Context.CourseCourseAttributes.AddRange(CourseAttributeIds.Select(caId => new CourseCourseAttribute
                {
                    CourseId = course.Id,
                    CourseAttributeId = caId
                }));

                await Context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}