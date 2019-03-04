using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Subjects
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Subject Subject { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subject = await _context.Subjects.FindAsync(id);

            if (Subject == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var subjectToUpdate = await _context.Subjects.FindAsync(id);

            if (await TryUpdateModelAsync<Subject>(
                subjectToUpdate,
                "Subject",
                s => s.Name))
            {
                // Check if any other subject has the same name
                if (await _context.Subjects.AnyAsync(s => s.Id != subjectToUpdate.Id && s.NormalizedName == subjectToUpdate.NormalizedName))
                {
                    ModelState.AddModelError(string.Empty, $"A subject already exists with the name {subjectToUpdate.Name}.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
