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
using CourseSchedulingSystem.Utilities;
using System.Collections.Async;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class EditModel : RoomPageModel
    {

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty]
        public Room Room { get; set; }

        [BindProperty] public RoomInputModel RoomModel { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.RoomRoomCapability)
                .ThenInclude(cat => cat.RoomCapability)
                .FirstOrDefaultAsync(m => m.Id == id);

            Room = await _context.Room.FirstOrDefaultAsync(m => m.Id == id);

            RoomModel = new RoomInputModel
            {
                Id = room.Id,
                RoomCapabilityIds = room.RoomRoomCapability.Select(cst => cst.RoomCapabilityId),
            };


            if (room == null)
            {
                return NotFound();
            }


            ViewData["BuildingId"] = new SelectList(_context.Building, "Id", "Code");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ViewData["BuildingId"] = new SelectList(_context.Building, "Id", "Code");
            _context.Attach(Room).State = EntityState.Modified;

            Room = await _context.Room.FirstOrDefaultAsync(m => m.Id == id);

            var room = await _context.Room
                .Include(r => r.RoomRoomCapability)
                .ThenInclude(cat => cat.RoomCapability)
                .FirstOrDefaultAsync(m => m.Id == id);

            // Update course attributes
            _context.UpdateManyToMany(Room.RoomRoomCapability,
                RoomModel.RoomCapabilityIds
                    .Select(caId => new RoomRoomCapability
                    {
                        RoomId = Room.Id,
                        RoomCapabilityId = caId
                    }),
            cca => cca.RoomCapabilityId);


            await room.DbValidateAsync(_context).ForEachAsync(result =>
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
            });

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool RoomExists(Guid id)
        {
            return _context.Room.Any(e => e.Id == id);
        }
    }
}
