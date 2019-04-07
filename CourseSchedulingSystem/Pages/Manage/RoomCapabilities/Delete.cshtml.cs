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
    public class DeleteModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public DeleteModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RoomCapability = await _context.RoomCapability.FindAsync(id);

            if (RoomCapability != null)
            {
                _context.RoomCapability.Remove(RoomCapability);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
