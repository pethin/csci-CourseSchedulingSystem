using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Buildings.Rooms
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
        
        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            Room = await _context.Rooms
                .Include(r => r.Building)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Room == null)
            {
                return NotFound();
            }
            
            if (returnUrl == null || !Url.IsLocalUrl(returnUrl))
            {
                ReturnUrl = Url.Page("../Details", new {id = Room.BuildingId});
            }
            else
            {
                ReturnUrl = returnUrl;
            }

            return Page();
        }
    }
}