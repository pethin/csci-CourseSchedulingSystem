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
using System.Collections.Async;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Room = await _context.Room
                .Include(r => r.Building)
                .Include(r => r.Capability).FirstOrDefaultAsync(m => m.Id == id);

            if (Room == null)
            {
                return NotFound();
            }
           ViewData["BuildingId"] = new SelectList(_context.Building, "Id", "Code");
           ViewData["CapabilityId"] = new SelectList(_context.Capability, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ViewData["BuildingId"] =
                new SelectList(_context.Building.Where(bd => bd.IsEnabled == true || bd.Id == Room.BuildingId), "Id",
                    "Code");
            ViewData["CapabilityId"] = new SelectList(_context.Capability, "Id", "Name");

            _context.Attach(Room).State = EntityState.Modified;


            var room = await _context.Room.FindAsync(id);

            if (await TryUpdateModelAsync(room, "Room", rm => rm.Number, rm => rm.BuildingId))
            {
                await room.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });


                if (!ModelState.IsValid) return Page();
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }

        private bool RoomExists(Guid id)
        {
            return _context.Room.Any(e => e.Id == id);
        }
    }
}
