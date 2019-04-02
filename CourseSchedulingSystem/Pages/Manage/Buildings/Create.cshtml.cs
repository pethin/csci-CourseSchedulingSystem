using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Buildings
{
    public class CreateModel : PageModel
    {
        private readonly CourseSchedulingSystem.Data.ApplicationDbContext _context;

        public CreateModel(CourseSchedulingSystem.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Building Building { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var newBuilding = new Building();

            if (await TryUpdateModelAsync(newBuilding, "Building", bd => bd.Name, bd => bd.Code))
            {
                //Check if any other building has the same name
                if(await _context.Building.AnyAsync(bd => bd.NormalizedName == newBuilding.NormalizedName))
                    ModelState.AddModelError(string.Empty, $"A building already exists with the name {newBuilding.Name}.");
                
                //Check if any other building has the same code
                if(await _context.Building.AnyAsync(bd => bd.Code == newBuilding.Code))
                    ModelState.AddModelError(string.Empty, $"A building already exists with the building code {newBuilding.Code}.");

                if (!ModelState.IsValid) return Page();
                
                _context.Building.Add(Building);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}