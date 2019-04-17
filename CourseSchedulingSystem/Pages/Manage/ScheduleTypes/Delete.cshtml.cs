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

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public ScheduleType ScheduleType { get; set; }

        public bool InUse => ScheduleType.CourseScheduleTypes.Any();

        public async Task<IActionResult> OnGetAsync()
        {
            ScheduleType = await _context.ScheduleTypes
                .Include(st => st.CourseScheduleTypes)
                .ThenInclude(cst => cst.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (ScheduleType == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ScheduleType = await _context.ScheduleTypes
                .Include(st => st.CourseScheduleTypes)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (ScheduleType != null)
            {
                if (InUse)
                {
                    return RedirectToPage();
                }

                _context.ScheduleTypes.Remove(ScheduleType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}