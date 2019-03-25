using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Instructors
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Instructor Instructor { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.Id == id);

            if (Instructor == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var instructor = await _context.Instructors.FindAsync(id);

            if (await TryUpdateModelAsync(
                instructor,
                "Instructor",
                i => i.FirstName, i => i.Middle, i => i.LastName))
            {
                // Check if any instructor has the same name
                if (await _context.Instructors.AnyAsync(i =>
                    i.Id != instructor.Id && i.NormalizedName == instructor.NormalizedName))
                    ModelState.AddModelError(string.Empty,
                        $"An instructor already exists with the name {instructor.FullName}.");

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}