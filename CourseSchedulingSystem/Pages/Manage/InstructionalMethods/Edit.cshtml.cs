using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.InstructionalMethods
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public InstructionalMethod InstructionalMethod { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            InstructionalMethod = await _context.InstructionalMethods.FirstOrDefaultAsync(m => m.Id == id);

            if (InstructionalMethod == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var instructionalMethod = await _context.InstructionalMethods.FindAsync(id);

            if (await TryUpdateModelAsync(
                instructionalMethod,
                "InstructionalMethod",
                im => im.Name, im => im.IsRoomRequired))
            {
                // Check if any instructional method has the same name
                if (await _context.InstructionalMethods.AnyAsync(im =>
                    im.Id != instructionalMethod.Id && im.NormalizedName == instructionalMethod.NormalizedName))
                    ModelState.AddModelError(string.Empty,
                        $"An instructional methods already exists with the name {instructionalMethod.Name}.");

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}