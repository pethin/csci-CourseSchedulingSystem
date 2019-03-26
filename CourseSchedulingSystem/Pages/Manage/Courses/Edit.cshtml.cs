using System;
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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject).FirstOrDefaultAsync(m => m.Id == id);

            if (Course == null) return NotFound();

            ViewData["DepartmentId"] = _context.Departments
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});

            ViewData["SubjectId"] = _context.Subjects
                .Select(d => new SelectListItem {Value = d.Id.ToString(), Text = $"{d.Code} - {d.Name}"});

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var courseToUpdate = await _context.Courses.FindAsync(id);

            if (await TryUpdateModelAsync(
                courseToUpdate,
                "Course",
                c => c.DepartmentId, c => c.SubjectId, c => c.Number, c => c.Title, c => c.CreditHours))
            {
                // Check if any course has the same subject and level
                if (await _context.Courses.AnyAsync(c =>
                    c.Id != courseToUpdate.Id && c.SubjectId == courseToUpdate.SubjectId && c.Number == courseToUpdate.Number))
                {
                    var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == courseToUpdate.SubjectId);

                    if (subject == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid subject selected.");
                        return Page();
                    }

                    courseToUpdate.Subject = subject;
                    ModelState.AddModelError(string.Empty,
                        $"A course already exists with the identifier {courseToUpdate.Identifier}.");
                }

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}