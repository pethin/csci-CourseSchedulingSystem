using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        public Term Term { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Term == null) return NotFound();
            return Page();
        }
    }
}