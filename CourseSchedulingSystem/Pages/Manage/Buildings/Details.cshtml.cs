using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Buildings
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        public Building Building { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Building = await _context.Buildings
                .Include(b => b.Rooms)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Building == null)
            {
                return NotFound();
            }
            
            return Page();
        }
    }
}
