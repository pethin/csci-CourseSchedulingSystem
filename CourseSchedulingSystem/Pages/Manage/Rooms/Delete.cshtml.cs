using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class DeleteModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public DeleteModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; }
        
        public bool InUse { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Room = await _context.Rooms
                .Include(r => r.Building)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Room == null)
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

            Room = await _context.Rooms.FindAsync(id);

            if (Room != null)
            {
                InUse = await InUseQueryAsync(id.Value);
                if (InUse)
                {
                    return Page();
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
