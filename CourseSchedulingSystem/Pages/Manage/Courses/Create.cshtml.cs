using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class CreateModel : CoursesPageModel
    {
        public CreateModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public Course Course { get; set; }

        public IActionResult OnGet()
        {
            LoadDropdownData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadDropdownData();

            if (!ModelState.IsValid) return Page();

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
                c => c.IsGraduate))
            {
                await course.DbValidateAsync(Context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                Context.Courses.Add(course);
                await Context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}