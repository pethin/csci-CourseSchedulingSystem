using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;

namespace CourseSchedulingSystem.Pages.Manage.RoomCapabilities
{
    public class DetailsModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public DetailsModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public RoomCapability RoomCapability { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RoomCapability = await _context.RoomCapability.FirstOrDefaultAsync(m => m.Id == id);

            if (RoomCapability == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
