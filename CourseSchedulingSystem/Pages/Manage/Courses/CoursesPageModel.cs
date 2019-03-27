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

        protected void LoadDropdownData()
        {
            ViewData["DepartmentId"] = Context.Departments
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});

            ViewData["SubjectId"] = Context.Subjects
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});
        }
    }
}