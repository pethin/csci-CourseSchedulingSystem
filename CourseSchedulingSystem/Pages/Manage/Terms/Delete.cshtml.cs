using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Term Term { get; set; }
        
        public bool HasCourseSections { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Term == null) return NotFound();

            HasCourseSections = await _context.CourseSections
                .Include(cs => cs.TermPart)
                .Where(cs => cs.TermPart.TermId == id)
                .AnyAsync();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Term != null)
            {
                _context.Terms.Remove(Term);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}