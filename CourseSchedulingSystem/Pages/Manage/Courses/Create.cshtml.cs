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
    public class CreateModel : CoursesPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public CourseInputModel CourseModel { get; set; }

        public IActionResult OnGet()
        {
            LoadDropdownData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadDropdownData();

            if (!ModelState.IsValid) return Page();

            var course = new Course
            {
                DepartmentId = CourseModel.DepartmentId,
                SubjectId = CourseModel.SubjectId,
                Number = CourseModel.Number,
                Title = CourseModel.Title,
                CreditHours = CourseModel.CreditHours,
                IsGraduate = CourseModel.CourseLevels.Contains(CourseLevelEnum.Graduate),
                IsUndergraduate = CourseModel.CourseLevels.Contains(CourseLevelEnum.Undergraduate)
            };

            await course.DbValidateAsync(Context).AddErrorsToModelState(ModelState);

            if (!ModelState.IsValid) return Page();

            Context.Courses.Add(course);
            await Context.SaveChangesAsync();

            var courseScheduleTypes = Context.ScheduleTypes
                .Where(st => CourseModel.ScheduleTypeIds.Contains(st.Id))
                .Select(st => new CourseScheduleType
                {
                    CourseId = course.Id,
                    ScheduleTypeId = st.Id
                });

            var courseAttributeTypes = Context.CourseAttributes
                .Where(at => CourseModel.CourseAttributeIds.Contains(at.Id))
                .Select(at => new CourseCourseAttribute
                {
                    CourseId = course.Id,
                    CourseAttributeId = at.Id
                });

            Context.CourseScheduleTypes.AddRange(courseScheduleTypes);
            Context.CourseCourseAttributes.AddRange(courseAttributeTypes);

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}