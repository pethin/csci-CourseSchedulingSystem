using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseSchedulingSystem.Pages.Manage.Buildings.Rooms
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid BuildingId { get; set; }

        [BindProperty] public Room Room { get; set; }

        public Building Building { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Building = await _context.Buildings.FindAsync(BuildingId);

            if (Building == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Building = await _context.Buildings.FindAsync(BuildingId);

            if (Building == null) return NotFound();

            if (!ModelState.IsValid) return Page();

            var room = new Room
            {
                BuildingId = Building.Id
            };

            if (await TryUpdateModelAsync(room, "Room", rm => rm.Number, rm => rm.Capacity))
            {
                await room.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                _context.Rooms.Add(room);
                
                await _context.SaveChangesAsync();
                return RedirectToPage("../Edit", new {id = BuildingId});
            }

            return Page();
        }
    }
}