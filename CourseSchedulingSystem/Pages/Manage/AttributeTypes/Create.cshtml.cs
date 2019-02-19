using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseSchedulingSystem.Pages.Manage.AttributeTypes
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

        [BindProperty] public AttributeType AttributeType { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var errors = (await AttributeType.ValidateAsync(_context)).Where(r => r != ValidationResult.Success);
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AttributeTypes.Add(AttributeType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}