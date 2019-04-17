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

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public Term Term { get; set; }
        
        public bool HasCourseSections { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Term == null) return NotFound();

            HasCourseSections = await _context.CourseSections
                .Include(cs => cs.TermPart)
                .Where(cs => cs.TermPart.TermId == Id)
                .AnyAsync();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Term != null)
            {
                _context.Terms.Remove(Term);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}