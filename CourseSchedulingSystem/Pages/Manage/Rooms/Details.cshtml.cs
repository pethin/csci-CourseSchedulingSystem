using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }

        public Room Room { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Room = await _context.Rooms
                .Include(r => r.Building).FirstOrDefaultAsync(m => m.Id == Id);

            if (Room == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}