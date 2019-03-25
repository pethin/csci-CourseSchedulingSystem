using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Course Course { get; set; }

        public IActionResult OnGet()
        {
            ViewData["DepartmentId"] = _context.Departments
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});

            ViewData["SubjectId"] = _context.Subjects
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var newCourse = new Course();

            if (await TryUpdateModelAsync(
                newCourse,
                "Course",
                c => c.DepartmentId, c => c.SubjectId, c => c.Level, c => c.Title, c => c.CreditHours))
            {
                // Check if any course has the same subject and level
                if (await _context.Courses.AnyAsync(c =>
                    c.SubjectId == newCourse.SubjectId && c.Level == newCourse.Level))
                {
                    var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == newCourse.SubjectId);

                    if (subject == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid subject selected.");
                        return Page();
                    }

                    newCourse.Subject = subject;
                    ModelState.AddModelError(string.Empty,
                        $"A course already exists with the identifier {newCourse.Identifier}.");
                }

                // Check if any course has the same title
                if (await _context.Courses.AnyAsync(c => c.Title == newCourse.Title))
                {
                    ModelState.AddModelError(string.Empty,
                        $"A course already exists with the title {newCourse.Title}.");
                }


                if (!ModelState.IsValid) return Page();

                _context.Courses.Add(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}