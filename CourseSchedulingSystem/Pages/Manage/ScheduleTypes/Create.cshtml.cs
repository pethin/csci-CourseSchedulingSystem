using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.ScheduleTypes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public ScheduleType ScheduleType { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var newScheduleType = new ScheduleType();

            if (await TryUpdateModelAsync(
                newScheduleType,
                "ScheduleType",
                st => st.Name))
            {
                // Check if any schedule type has the same name
                if (await _context.ScheduleTypes.AnyAsync(st => st.NormalizedName == newScheduleType.NormalizedName))
                    ModelState.AddModelError(string.Empty,
                        $"A schedule type already exists with the name {newScheduleType.Name}.");

                if (!ModelState.IsValid) return Page();

                _context.ScheduleTypes.Add(newScheduleType);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}