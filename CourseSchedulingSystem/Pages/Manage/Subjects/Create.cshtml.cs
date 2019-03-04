using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Subjects
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
        public Subject Subject { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newSubject = new Subject();

            if (await TryUpdateModelAsync<Subject>(
                newSubject,
                "Subject",
                s => s.Code, s => s.Name))
            {
                // Check if any subject has the same code
                if (await _context.Subjects.AnyAsync(s => s.Code == newSubject.Code))
                {
                    ModelState.AddModelError(string.Empty, $"A subject already exists with the code {newSubject.Code}.");
                }

                // Check if any subject has the same name
                if (await _context.Subjects.AnyAsync(s => s.NormalizedName == newSubject.NormalizedName))
                {
                    ModelState.AddModelError(string.Empty, $"A subject already exists with the name {newSubject.Name}.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                _context.Subjects.Add(newSubject);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}