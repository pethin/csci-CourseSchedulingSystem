using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public Room Room { get; set; }

        public bool InUse { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Room = await _context.Rooms
                .Include(r => r.Building)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Room == null)
            {
                return NotFound();
            }

            InUse = await InUseQueryAsync(Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Room = await _context.Rooms.FindAsync(Id);

            if (Room != null)
            {
                InUse = await InUseQueryAsync(Id);
                if (InUse)
                {
                    return RedirectToPage();
                }

                _context.Rooms.Remove(Room);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> InUseQueryAsync(Guid id)
        {
            return await _context.Rooms
                .Where(r => r.Id == id)
                .Where(r => r.ScheduledMeetingTimeRooms.Any())
                .AnyAsync();
        }
    }
}