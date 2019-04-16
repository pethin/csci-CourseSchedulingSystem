using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Buildings
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Building Building { get; set; }

        public bool InUse { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Building = await _context.Buildings.FirstOrDefaultAsync(m => m.Id == id);

            if (Building == null)
            {
                return NotFound();
            }

            InUse = await InUseQueryAsync(id.Value);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Building = await _context.Buildings.FindAsync(id);

            if (Building != null)
            {
                InUse = await InUseQueryAsync(id.Value);
                if (InUse)
                {
                    return Page();
                }
                
                _context.Buildings.Remove(Building);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> InUseQueryAsync(Guid id)
        {
            return await _context.Buildings
                .Where(b => b.Id == id)
                .Where(b => b.Rooms.Any(r => r.ScheduledMeetingTimeRooms.Any()))
                .AnyAsync();
        }
    }
}
