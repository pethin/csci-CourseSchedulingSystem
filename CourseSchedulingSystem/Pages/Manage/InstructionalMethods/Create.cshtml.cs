using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.InstructionalMethods
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
        public InstructionalMethod InstructionalMethod { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var newInstructionalMethod = new InstructionalMethod();

            if (await TryUpdateModelAsync(
                newInstructionalMethod,
                "InstructionalMethod",
                im => im.Name, im => im.IsRoomRequired))
            {
                // Check if any instructional method has the same name
                if (await _context.InstructionalMethods.AnyAsync(im => im.NormalizedName == newInstructionalMethod.NormalizedName))
                    ModelState.AddModelError(string.Empty,
                        $"An instructional methods already exists with the name {newInstructionalMethod.Name}.");

                if (!ModelState.IsValid) return Page();

                _context.InstructionalMethods.Add(newInstructionalMethod);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}