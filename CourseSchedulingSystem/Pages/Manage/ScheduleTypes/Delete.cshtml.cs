using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.ScheduleTypes
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public ScheduleType ScheduleType { get; set; }

        public bool InUse => ScheduleType.CourseScheduleTypes.Any();

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ScheduleType = await _context.ScheduleTypes
                .Include(st => st.CourseScheduleTypes)
                .ThenInclude(cst => cst.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ScheduleType == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ScheduleType = await _context.ScheduleTypes
                .Include(st => st.CourseScheduleTypes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ScheduleType != null)
            {
                if (InUse)
                {
                    return Page();
                }

                _context.ScheduleTypes.Remove(ScheduleType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}