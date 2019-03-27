using System;
using System.Collections.Async;
using System.Collections.Generic;
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

        [BindProperty] public CourseInputModel CourseModel { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            var course = await Context.Courses
                .Include(c => c.CourseScheduleTypes)
                .Include(c => c.CourseAttributeTypes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound();

            CourseModel = new CourseInputModel
            {
                Id = course.Id,
                DepartmentId = course.DepartmentId,
                SubjectId = course.SubjectId,
                Title = course.Title,
                Number = course.Number,
                CreditHours = course.CreditHours,
                ScheduleTypeIds = course.CourseScheduleTypes.Select(cst => cst.ScheduleTypeId),
                CourseAttributeIds = course.CourseAttributeTypes.Select(cat => cat.AttributeTypeId)
            };

            var courseLevels = new List<CourseLevelEnum>();
            if (course.IsUndergraduate) courseLevels.Add(CourseLevelEnum.Undergraduate);
            if (course.IsGraduate) courseLevels.Add(CourseLevelEnum.Graduate);

            CourseModel.CourseLevels = courseLevels;

            LoadDropdownData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            LoadDropdownData();

            if (!ModelState.IsValid) return Page();

            var course = await Context.Courses
                .Include(c => c.CourseScheduleTypes)
                .Include(c => c.CourseAttributeTypes)
                .FirstOrDefaultAsync(m => m.Id == id);

            course.DepartmentId = CourseModel.DepartmentId;
            course.SubjectId = CourseModel.SubjectId;
            course.Number = CourseModel.Number;
            course.Title = CourseModel.Title;
            course.CreditHours = CourseModel.CreditHours;
            course.IsGraduate = CourseModel.CourseLevels.Contains(CourseLevelEnum.Graduate);
            course.IsUndergraduate = CourseModel.CourseLevels.Contains(CourseLevelEnum.Undergraduate);

            await course.DbValidateAsync(Context).ForEachAsync(result =>
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
            });

            if (!ModelState.IsValid) return Page();

            await Context.SaveChangesAsync();

            // Update schedule types
            Context.UpdateManyToMany(course.CourseScheduleTypes,
                CourseModel.ScheduleTypeIds
                    .Select(stId => new CourseScheduleType
                    {
                        CourseId = course.Id,
                        ScheduleTypeId = stId
                    }),
                cst => cst.ScheduleTypeId);

            // Update course attributes
            Context.UpdateManyToMany(course.CourseAttributeTypes,
                CourseModel.CourseAttributeIds
                    .Select(caId => new CourseAttributeType
                    {
                        CourseId = course.Id,
                        AttributeTypeId = caId
                    }),
                cat => cat.AttributeTypeId);

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}