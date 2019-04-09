using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CourseSchedulingSystem.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class CoursesPageModel : PageModel
    {
        protected readonly ApplicationDbContext Context;

        public CoursesPageModel(ApplicationDbContext context)
        {
            Context = context;
        }

        public IEnumerable<SelectListItem> ScheduleTypeOptions =>
            Context.ScheduleTypes.Select(st => new SelectListItem
            {
                Value = st.Id.ToString(),
                Text = st.Name
            });

        public IEnumerable<SelectListItem> CourseAttributeOptions =>
            Context.CourseAttributes.Select(st =>
                new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name
                });

        public IEnumerable<SelectListItem> DepartmentOptions =>
            Context.Departments.Select(d =>
                new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.Code} - {d.Name}"
                });

        public IEnumerable<SelectListItem> SubjectOptions =>
            Context.Subjects.Select(s =>
                new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Code} - {s.Name}"
                });
    }
}