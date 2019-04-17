using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
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

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public ScheduleType ScheduleType { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ScheduleType = await _context.ScheduleTypes.FirstOrDefaultAsync(m => m.Id == Id);

            if (ScheduleType == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var scheduleType = await _context.ScheduleTypes.FindAsync(Id);

            if (await TryUpdateModelAsync(
                scheduleType,
                "ScheduleType",
                st => st.Code,
                st => st.Name))
            {
                await scheduleType.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}