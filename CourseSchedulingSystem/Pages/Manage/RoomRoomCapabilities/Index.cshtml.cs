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
    public class IndexModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public IndexModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<RoomRoomCapability> RoomRoomCapability { get;set; }

        public async Task OnGetAsync()
        {
            RoomRoomCapability = await _context.RoomRoomCapabilities
                .Include(r => r.Room)
                .Include(r => r.RoomCapability).ToListAsync();
        }
    }
}
