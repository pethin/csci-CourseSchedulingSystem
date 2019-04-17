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

        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public Building Building { get; set; }

        public bool InUse { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Building = await _context.Buildings.FirstOrDefaultAsync(m => m.Id == Id);

            if (Building == null)
            {
                return NotFound();
            }

            InUse = await InUseQueryAsync(Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Building = await _context.Buildings.FindAsync(Id);

            if (Building != null)
            {
                InUse = await InUseQueryAsync(Id);
                if (InUse)
                {
                    return RedirectToPage();
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