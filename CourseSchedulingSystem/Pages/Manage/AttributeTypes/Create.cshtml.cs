using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

            var newAttributeType = new AttributeType();

            if (await TryUpdateModelAsync<AttributeType>(
                newAttributeType,
                "AttributeType",
                at => at.Name))
            {
                // Check if any attribute type has the same name
                if (await _context.AttributeTypes.AnyAsync(at => at.NormalizedName == newAttributeType.NormalizedName))
                {
                    ModelState.AddModelError(string.Empty, $"An attribute type already exists with the name {newAttributeType.Name}.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                _context.AttributeTypes.Add(newAttributeType);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}