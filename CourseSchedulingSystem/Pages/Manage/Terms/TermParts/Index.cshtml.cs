using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Terms.TermParts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Term Term { get; set; }

        public IList<TermPart> TermPart { get;set; }

        public async Task<IActionResult> OnGetAsync(Guid? termId)
        {
            if (termId == null) return NotFound();

            Term = await _context.Terms
                .Include(t => t.TermParts)
                .FirstOrDefaultAsync(t => t.Id == termId);

            if (Term == null) return NotFound();

            TermPart = Term.TermParts.ToList();
            return Page();
        }
    }
}
