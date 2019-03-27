using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
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

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Course = await Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Course == null) return NotFound();

            LoadDropdownData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            LoadDropdownData();

            if (!ModelState.IsValid) return Page();

            var course = await Context.Courses.FindAsync(id);

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

                await Context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}