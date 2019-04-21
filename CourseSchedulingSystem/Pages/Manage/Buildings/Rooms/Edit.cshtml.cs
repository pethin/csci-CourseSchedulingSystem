using System;
using System.Collections.Async;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Buildings.Rooms
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public Room Room { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Room = await _context.Rooms
                .Include(r => r.Building)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Room == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var room = await _context.Rooms
                .Include(r => r.Building)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (await TryUpdateModelAsync(room, "Room", rm => rm.IsEnabled, rm => rm.Number, rm => rm.Capacity))
            {
                await room.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();
                await _context.SaveChangesAsync();
                
                return RedirectToPage("../Edit", new {id = room.BuildingId});
            }

            return Page();
        }
    }
}