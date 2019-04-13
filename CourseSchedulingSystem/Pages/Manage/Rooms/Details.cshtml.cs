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
    public class DetailsModel : RoomPageModel
    {

        public DetailsModel(CourseSchedulingSystem.Data.ApplicationDbContext context) : base(context)
        {
        }

        public Room Room { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Room = await _context.Rooms
                .Include(r => r.Building)
                .Include(r => r.RoomRoomCapability)
                .ThenInclude(cat => cat.RoomCapability)
                .FirstOrDefaultAsync(m => m.Id == id); 

            if (Room == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
