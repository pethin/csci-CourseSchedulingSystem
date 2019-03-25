using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Instructors
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Instructor Instructor { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var instructor = new Instructor();

            if (await TryUpdateModelAsync(
                instructor,
                "Instructor",
                i => i.FirstName, i => i.Middle, i => i.LastName))
            {
                // Check if any instructor has the same name
                if (await _context.Instructors.AnyAsync(i => i.NormalizedName == instructor.NormalizedName))
                    ModelState.AddModelError(string.Empty,
                        $"An instructor already exists with the name {instructor.FullName}.");

                if (!ModelState.IsValid) return Page();

                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}