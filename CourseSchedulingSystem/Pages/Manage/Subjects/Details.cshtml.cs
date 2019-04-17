using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Subjects
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        public Subject Subject { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Subject = await _context.Subjects
                .Include(s => s.Courses)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Subject == null) return NotFound();
            return Page();
        }
    }
}