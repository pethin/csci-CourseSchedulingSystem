using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.ScheduleTypes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public ScheduleType ScheduleType { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ScheduleType = await _context.ScheduleTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (ScheduleType == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid) return Page();

            var scheduleTypeToUpdate = await _context.ScheduleTypes.FindAsync(id);

            if (await TryUpdateModelAsync(
                scheduleTypeToUpdate,
                "ScheduleType",
                st => st.Name))
            {
                // Check if any other schedule type has the same name
                if (await _context.ScheduleTypes.AnyAsync(at =>
                    at.Id != scheduleTypeToUpdate.Id && at.NormalizedName == scheduleTypeToUpdate.NormalizedName))
                    ModelState.AddModelError(string.Empty,
                        $"A schedule type already exists with the name {scheduleTypeToUpdate.Name}.");

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}