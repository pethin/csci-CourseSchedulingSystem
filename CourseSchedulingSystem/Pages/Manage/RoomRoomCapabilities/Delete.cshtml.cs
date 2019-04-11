using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.RoomRoomCapabilities
{
    public class DeleteModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public DeleteModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RoomRoomCapability RoomRoomCapability { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RoomRoomCapability = await _context.RoomRoomCapability
                .Include(r => r.Room)
                .Include(r => r.RoomCapability).FirstOrDefaultAsync(m => m.RoomId == id);

            if (RoomRoomCapability == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RoomRoomCapability = await _context.RoomRoomCapability.FindAsync(id);

            if (RoomRoomCapability != null)
            {
                _context.RoomRoomCapability.Remove(RoomRoomCapability);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
