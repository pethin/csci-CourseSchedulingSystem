using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms.TermParts
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public TermPart TermPart { get; set; }

        public bool InUse => false;

        public async Task<IActionResult> OnGetAsync(Guid? termId, Guid? id, string returnUrl = null)
        {
            if (termId == null || id == null)
            {
                return NotFound();
            }

            TermPart = await _context.TermParts
                .Include(t => t.Term)
                .Where(tp => tp.TermId == termId)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (TermPart == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? termId, Guid? id, string returnUrl = null)
        {
            if (termId == null || id == null) return NotFound();

            TermPart = await _context.TermParts
                .Include(t => t.Term)
                .Where(tp => tp.TermId == termId)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (TermPart == null) return NotFound();

            if (InUse)
            {
                return Page();
            }

            _context.TermParts.Remove(TermPart);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Edit", new {id = TermPart.TermId});
        }
    }
}