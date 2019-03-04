using System;
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

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ScheduleType = await _context.ScheduleTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (ScheduleType == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null) return NotFound();

            ScheduleType = await _context.ScheduleTypes.FindAsync(id);

            if (ScheduleType != null)
            {
                _context.ScheduleTypes.Remove(ScheduleType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}