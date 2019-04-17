using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Buildings
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }

        [BindProperty] public Building Building { get; set; }
        
        public string OriginalBuildingName { get; set; }
        public IList<Room> Rooms { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Building = await _context.Buildings.FirstOrDefaultAsync(m => m.Id == Id);

            if (Building == null)
            {
                return NotFound();
            }

            OriginalBuildingName = Building.Name;
            Rooms = await _context.Rooms
                .Where(r => r.BuildingId == Id)
                .OrderBy(r => r.Number)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var building = await _context.Buildings.FindAsync(Id);

            if (building == null) return NotFound();

            OriginalBuildingName = building.Name;
            Rooms = await _context.Rooms
                .Where(r => r.BuildingId == Id)
                .OrderBy(r => r.Number)
                .ToListAsync();

            if (await TryUpdateModelAsync(building, "Building", bd => bd.Code, bd => bd.Name))
            {
                await building.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();
                await _context.SaveChangesAsync();

                OriginalBuildingName = building.Name;
                SuccessMessage = "Building successfully updated!";
                return Page();
            }

            return Page();
        }
    }
}