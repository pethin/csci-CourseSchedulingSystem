using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.RoomRoomCapabilities
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
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
           ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Number");
           ViewData["RoomCapabilityId"] = new SelectList(_context.RoomCapability, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(RoomRoomCapability).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomRoomCapabilityExists(RoomRoomCapability.RoomId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RoomRoomCapabilityExists(Guid id)
        {
            return _context.RoomRoomCapability.Any(e => e.RoomId == id);
        }
    }
}
