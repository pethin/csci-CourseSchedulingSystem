using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseSchedulingSystem.Pages.Manage.InstructionalMethods
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public InstructionalMethod InstructionalMethod { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var instructionalMethod = new InstructionalMethod();

            if (await TryUpdateModelAsync(
                instructionalMethod,
                "InstructionalMethod",
                im => im.Code,
                im => im.Name,
                im => im.IsRoomRequired))
            {
                await instructionalMethod.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                _context.InstructionalMethods.Add(instructionalMethod);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}