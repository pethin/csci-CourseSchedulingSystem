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

namespace CourseSchedulingSystem.Pages.Manage.Buildings
{
    public class EditModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public EditModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Building Building { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Building = await _context.Building.FirstOrDefaultAsync(m => m.Id == id);

            if (Building == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var buildingToUpdate = await _context.Building.FindAsync(id);

            if (await TryUpdateModelAsync(buildingToUpdate, "Building", bd => bd.Code, bd => bd.Name))
            {
                //Check if any other building has the same name
                if (await _context.Building.AnyAsync(bd =>
                    bd.Id != buildingToUpdate.Id && bd.NormalizedName == buildingToUpdate.NormalizedName))
                    ModelState.AddModelError(string.Empty,
                        $"A building already exists with the name {buildingToUpdate.Name}.");

                //Check if any other building has the same code
                if (await _context.Building.AnyAsync(bd =>
                    bd.Id != buildingToUpdate.Id && bd.Code == buildingToUpdate.Code))
                    ModelState.AddModelError(string.Empty,
                        $"A building already exists with the building code {buildingToUpdate.Code}.");

                if (!ModelState.IsValid) return Page();
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
